import './index.scss';

(function ($) {
    "use strict";

    // Add active state to sidbar nav links
    var path = window.location.href; // because the 'href' property of the DOM element is the absolute path
    $("#layoutSidenav_nav .sb-sidenav a.nav-link").each(function () {
        if (this.href === path) {
            $(this).addClass("active");
        }
    });
    // Toggle the side navigation
    $(document).on("click", "#sidebarToggle", function (e) {
        e.preventDefault();
        $("body").toggleClass("sb-sidenav-toggled");
    });
})(jQuery);

window.editors = {};

window["BlazorCKE"] = {
    init: function (params) {
        ClassicEditor
            .create(document.querySelector('#' + params.selector), {})
            .then(editor => {
                window.editors[params.selector] = editor;
                editor.setData(params.content ? params.content : '');
                editor.model.document.on('change:data', () => {
                    params.instance.invokeMethodAsync('UpdateText', editor.getData());
                });
            })
            .catch(error => {
                console.error(error);
            });
    },
    destroy: function (params) {
        if (window.editors.hasOwnProperty(params.selector)) {
            window.editors[params.selector].destroy();
            delete window.editors[params.selector];
        }
    }
};


window.twttr = (function (d, s, id) {
    var js, fjs = d.getElementsByTagName(s)[0],
        t = window.twttr || {};
    if (d.getElementById(id)) return t;
    js = d.createElement(s);
    js.id = id;
    js.src = "https://platform.twitter.com/widgets.js";
    fjs.parentNode.insertBefore(js, fjs);
    t._e = [];
    t.ready = function (f) {
        t._e.push(f);
    };
    return t;
}(document, "script", "twitter-wjs"));

window.BlazorTwttr = {
    render: function (params) {
        console.log('render tweet', params);
        const container = document.getElementById('twitter-' + params.id);
        container.innerHTML = '';
        twttr.widgets.createTweet(
            params.tweetId + '', container,
            {
                theme: 'dark'
            }
        );
    }
};

window.BlazorTwitch = {
    render: function (params) {
        console.log('render twitch', params);
        const container = document.getElementById('twitch-' + params.id);
        container.innerHTML = '';
        const twitchParams = {
            width: 854,
            height: 480,
            layout: 'video',
            autoplay: false
        };
        if (params.video) {
            twitchParams.video = params.video;
        } else if (params.channel) {
            twitchParams.channel = params.channel;
        } else if (params.collection) {
            twitchParams.collection = params.collection;
        }
        new Twitch.Player('twitch-' + params.id, twitchParams);
    }
};

window.FileUpload = {
    open: function (params) {
        const container = document.getElementById(params.id);
        container.click();
    }
};
