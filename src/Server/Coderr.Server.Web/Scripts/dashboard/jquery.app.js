toggleMenuItem = function (uri) {
    if (window.location.pathname === window.WEB_ROOT && window.location.hash === '')
        uri = window.WEB_ROOT + '#/';
    var $a;
    if (typeof uri === 'string') {

        // we do not include root in links since
        // we can exist in a virtual folder, which means 
        // that we would have had to parse links on all regular HTML pages.
        var webRoot = window['WEB_ROOT'];
        if (uri.indexOf(webRoot) === 0) {
            uri = uri.substr(webRoot.length);
        }
        $a = $('#sidebar-menu a[href="' + uri + '"]');
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
    });

$('#sidebar-menu').on('click',
    '.has_sub > a',
    function() {
        var b = $(this).next();
        var a = b.find('a');
        window.location = $(a[0]).attr('href');
    });

