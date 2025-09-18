using System.ComponentModel.DataAnnotations;
using FormSite.Data;
using FormSite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FormSite.Pages;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;
    public IndexModel(AppDbContext db) => _db = db;

    public List<Option> Available { get; private set; } = new();

    [BindProperty, Required(ErrorMessage = "Selecione pelo menos um item.")]
    public List<int> SelectedOptionIds { get; set; } = new();

    // Identificador simples do usu�rio para exemplo (na pr�tica, use login/claims/cookie)
    private string UserToken => HttpContext.Connection.RemoteIpAddress?.ToString() ?? Guid.NewGuid().ToString();

    public async Task OnGet()
    {
        // Op��es ainda n�o reservadas
        var reservedIds = await _db.Reservations.Select(r => r.OptionId).ToListAsync();

        Available = await _db.Options
            .Where(o => !reservedIds.Contains(o.Id))
            .OrderBy(o => o.Name)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await OnGet();
            return Page();
        }

        // Tenta reservar todas em uma transa��o
        using var tx = await _db.Database.BeginTransactionAsync();

        try
        {
            var alreadyReserved = await _db.Reservations
                .Where(r => SelectedOptionIds.Contains(r.OptionId))
                .Select(r => r.OptionId)
                .ToListAsync();

            if (alreadyReserved.Any())
            {
                ModelState.AddModelError(string.Empty, "Alguns itens j� foram selecionados por outra pessoa.");
                await OnGet();
                return Page();
            }

            foreach (var optionId in SelectedOptionIds)
            {
                _db.Reservations.Add(new Reservation
                {
                    OptionId = optionId,
                    UserToken = UserToken
                });
            }

            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            return RedirectToPage("ThankYou");
        }
        catch (DbUpdateException)
        {
            // Pega colis�o de UNIQUE (se duas pessoas clicarem quase juntas)
            await tx.RollbackAsync();
            ModelState.AddModelError(string.Empty, "Conflito: algum item ficou indispon�vel durante o envio.");
            await OnGet();
            return Page();
        }
    }
}
