toggleMenuItem = function (uri) {

    var webRoot = window.WEB_ROOT.toLocaleLowerCase();

    if (window.location.pathname.toLocaleLowerCase() === webRoot && window.location.hash === '')
        uri = window.WEB_ROOT + '#/';
    var $a;
    if (typeof uri === 'string') {

        // we do not include root in links since
        // we can exist in a virtual folder, which means 
        // that we would have had to parse links on all regular HTML pages.
        uri = uri.toLocaleLowerCase();
        if (uri.indexOf(webRoot) === 0) {
            uri = uri.substr(webRoot.length);
        }

        var uriToFind = uri;
        var selector = '#sidebar-menu a[href="' + uri + '"]';
        $a = $(selector);

        //try to append trailing slash
        if ($a.length === 0) {
            $a = $('#sidebar-menu a[href="' + uri + '/"]');
        }

        // divide the url until we find a parent menu item
        // (since some pages only exist in context menus)
        while ($a.length === 0) {
            var pos = uriToFind.lastIndexOf('/');
            if (pos === uriToFind.length - 1) {
                uriToFind = uriToFind.substr(0, uriToFind.length - 1);
                pos = uriToFind.lastIndexOf('/');
            }
            if (pos === -1) {
                break;
            }

            uriToFind = uriToFind.substr(0, pos + 1);
            $a = $('#sidebar-menu a[href="' + uriToFind + '"]');
        }
    } else {
        $a = $(uri);
    }
    $('#sidebar-menu ul').not(':first').css('display', 'none');
    $('#sidebar-menu .active').removeClass('active');
    $a.parents('li').addClass('active');
    $a.parents('ul').prev('a').addClass('active');
    $a.parents('ul').css('display', 'block');
}

window.addEventListener('hashchange',
    function() {
        window.scrollTo(0, 0);
        toggleMenuItem(window.location.pathname + window.location.hash);
    });


$('#sidebar-menu').on('click',
    '.has_sub > a',
    function() {
        var b = $(this).next();
        var a = b.find('a');
        window.location = $(a[0]).attr('href');
    });

