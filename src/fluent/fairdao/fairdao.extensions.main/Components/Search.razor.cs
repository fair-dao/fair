using Microsoft.FluentUI.AspNetCore.Components;

namespace fairdao.extensions.main.Components;

public partial class Search
{
    private IDialogReference? _dialog;

    private async Task SearchAsync()
    {
        _dialog = await DialogService.ShowPanelAsync<SearchPanel>(new DialogParameters()
        {
            ShowTitle = true,
            Title = Localer["≤È’“"],
            Alignment = HorizontalAlignment.Right,
            PrimaryAction = Localer["OK"],
            SecondaryAction = null,
            ShowDismiss = true
        });

        DialogResult result = await _dialog.Result;
    }
}
