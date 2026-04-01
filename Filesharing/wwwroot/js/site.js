/**
 * FileSharing - Premium JS Core
 */

(function($) {
    "use strict";

    // 1. Notification System (Glass Toasts)
    window.Notify = {
        show: function(message, type = 'success') {
            const container = $('#glass-toast-container');
            const id = 'toast-' + Date.now();
            const icon = type === 'success' ? 'bi-check-circle-fill' : 'bi-exclamation-triangle-fill';
            const title = type.toUpperCase();

            const toastHtml = `
                <div id="${id}" class="glass-toast ${type}">
                    <div class="glass-toast-icon">
                        <i class="bi ${icon}"></i>
                    </div>
                    <div class="glass-toast-content">
                        <b class="glass-toast-title">${title}</b>
                        <div class="glass-toast-message">${message}</div>
                    </div>
                    <button class="glass-toast-close" onclick="Notify.hide('${id}')">
                        <i class="bi bi-x-lg"></i>
                    </button>
                </div>
            `;

            container.append(toastHtml);
            const toast = $('#' + id);
            
            // Trigger animation
            setTimeout(() => toast.addClass('show'), 100);

            // Auto hide
            setTimeout(() => this.hide(id), 5000);
        },

        hide: function(id) {
            const toast = $('#' + id);
            toast.addClass('hide');
            setTimeout(() => toast.remove(), 600);
        },

        checkCookies: function() {
            const msg = this.getCookie('fs_msg');
            const type = this.getCookie('fs_type');

            if (msg) {
                this.show(decodeURIComponent(msg.replace(/\+/g, ' ')), type || 'success');
                this.deleteCookie('fs_msg');
                this.deleteCookie('fs_type');
            }
        },

        getCookie: function(name) {
            let nameEQ = name + "=";
            let ca = document.cookie.split(';');
            for(let i=0;i < ca.length;i++) {
                let c = ca[i];
                while (c.charAt(0)==' ') c = c.substring(1,c.length);
                if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length,c.length);
            }
            return null;
        },

        deleteCookie: function(name) {
            document.cookie = name + '=; Max-Age=-99999999; path=/';
        }
    };

    $(document).ready(function() {
        // Create container if not exists
        if ($('#glass-toast-container').length === 0) {
            $('body').append('<div id="glass-toast-container" class="glass-toast-container"></div>');
        }

        // Check for server-side notifications in cookies
        Notify.checkCookies();

        // Lazyload
        if (typeof lazyload === 'function') {
            lazyload();
        }

        // Preloader
        $('#mu-status').fadeOut();
        $('#mu-preloader').delay(300).fadeOut('slow');

        // Scroll to top
        $(window).scroll(function() {
            if ($(this).scrollTop() > 300) {
                $('.scrollToTop').addClass('show');
            } else {
                $('.scrollToTop').removeClass('show');
            }
        });

        $('.scrollToTop').on('click', function() {
            $('html, body').animate({scrollTop: 0}, 800);
            return false;
        });
    });

})(jQuery);
