using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components.Extensions;

namespace fairdao.extensions.main.Components;

public partial class SearchPanel
{
    public string KeyType { get; set; }

    fairdao.extensions.shared.Extender CurExtender;

    protected override Task OnInitializedAsync()
    {
        return base.OnInitializedAsync();
    }

    void SelectKeyType(fairdao.extensions.shared.Extender ext, string keyType)
    {
        this.CurExtender = ext;
        this.KeyType = keyType;
    }
}
