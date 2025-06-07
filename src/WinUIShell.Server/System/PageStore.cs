using Microsoft.UI.Xaml.Navigation;
using WinUIShell.Common;

namespace WinUIShell.Server;

internal sealed class PageStore : Singleton<PageStore>
{
    public sealed class PageProperty
    {
        public string Name { get; set; } = "";
        public Type? Type { get; set; }
        public int CallbackQueueThreadId { get; set; }
        public NavigationCacheMode NavigationCacheMode { get; set; }
    }

    private readonly int _maxPageCount;
    private readonly Queue<Type> _remainingPageTypes = [];
    private readonly Dictionary<string, PageProperty> _assignedPages = [];
    private readonly Dictionary<string, IPage> _pageInstances = [];

    public PageStore()
    {
        Type[] pageTypes = [
            typeof(Page01),
            typeof(Page02),
            typeof(Page03),
            typeof(Page04),
            typeof(Page05),
            typeof(Page06),
            typeof(Page07),
            typeof(Page08),
            typeof(Page09),
            typeof(Page10),
        ];
        _maxPageCount = pageTypes.Length;

        foreach (Type pageType in pageTypes)
        {
            _remainingPageTypes.Enqueue(pageType);
        }
    }

    public Type RegisterPageProperty(string pageName, int callbackQueueThreadId, NavigationCacheMode navigationCacheMode)
    {
        lock (_remainingPageTypes)
        {
            if (_assignedPages.TryGetValue(pageName, out var pageProperty))
            {
                pageProperty.CallbackQueueThreadId = callbackQueueThreadId;
                pageProperty.NavigationCacheMode = navigationCacheMode;
                return pageProperty.Type!;
            }

            if (_remainingPageTypes.TryDequeue(out Type? newPageType))
            {
                _assignedPages[pageName] = new PageProperty
                {
                    Name = pageName,
                    Type = newPageType,
                    CallbackQueueThreadId = callbackQueueThreadId,
                    NavigationCacheMode = navigationCacheMode
                };
                return newPageType;
            }

            throw new InvalidOperationException($"Page allocation failed [{pageName}] due to running out of Pages. Maximum page count available is [{_maxPageCount}].");
        }
    }

    public PageProperty GetPageProperty(Type pageType)
    {
        lock (_remainingPageTypes)
        {
            foreach (var it in _assignedPages)
            {
                if (it.Value.Type == pageType)
                {
                    return it.Value;
                }
            }

            throw new InvalidOperationException($"Page name not found for type [{pageType}].");
        }
    }

    public void RegisterPageInstance<TPage>(TPage page) where TPage : IPage
    {
        var property = GetPageProperty(typeof(TPage));

        lock (_remainingPageTypes)
        {
            if (_pageInstances.TryGetValue(property.Name, out IPage? previousPage))
            {
                // Destroy previous page instance.
                CommandClient.Get().DestroyObject(previousPage.Id);
                previousPage.Id = new();
            }

            _pageInstances[property.Name] = page;
        }
    }

}
