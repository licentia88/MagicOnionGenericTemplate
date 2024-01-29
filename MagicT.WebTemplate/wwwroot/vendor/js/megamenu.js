jQuery(window).on("load", function () {
    "use strict";

(function(){
	'use strict';
	if($('.top-bar').length>0)
		var t = $('.top-bar').height();
	else t=0;
	$('.megamenu .arrow').on("click", function (){

		if($(this).parent().hasClass('hover')){
			$(this).parent().removeClass("hover");
            $('.sub-arrow').toggleClass('inner-arrow');

        }else{
			$(this).parent().addClass("hover");
            $('.sub-arrow').toggleClass('inner-arrow');

        }

	});

	var k=0;
	$(window).on('scroll', function (){
		if($(window).width()>1000){
			if($(window).scrollTop()>200+t){
				$('.megamenu').removeAttr('style').addClass('pin');
			}else{

				$('.megamenu').css({top:-$(window).scrollTop()}).removeClass('pin');
			}if($(window).scrollTop()>150+t){
				$('.megamenu').addClass('before');
			}else{

				$('.megamenu').removeClass('before');
			}

		}else{

			//$('.megamenu').css({top:$(window).scrollTop()})
			if($(window).scrollTop()<k){
				$('.megamenu').addClass('off').removeClass('woff').removeAttr('style');
				$('#menu').removeClass('in');
				k=0;
			}
		}
		if($(window).scrollTop()>t){
			if(!$('.megamenu').hasClass('woff')){
				$('.megamenu').addClass('pin-start').addClass('off');
			}

		}else{
			$('.megamenu').removeClass('pin-start').removeClass('off');
		}
	});
	if($(window).scrollTop()>150+t){
		$('.megamenu').addClass('pin');
	}else{
		$('.megamenu').removeAttr('style').removeClass('pin');
	}
	$(window).on("resize", function () {
		if($(window).width()>1000){
			$('.megamenu').removeAttr('style');
		}
	});
	if($(window).scrollTop()>t){
		$('.megamenu').addClass('off').addClass('pin-start');
	}else{
		$('.megamenu').removeClass('off').removeClass('pin-start');
	}
	$('.menu-icon').on("click", function (){
		if($('#menu').hasClass('in')){
			$('.megamenu').addClass('off').removeClass('woff').removeAttr('style');
			if($(window).scrollTop()>t){
				if(!$('.megamenu').hasClass('woff')){
					$('.megamenu').addClass('pin-start').addClass('off');
				}

			}else{
				$('.megamenu').removeClass('pin-start').removeClass('off');
			}

		}else{
			k=$(window).scrollTop();
			$('.megamenu').removeClass('off').addClass('woff').css({top:$(window).scrollTop()});
		}
	})

})();

    jQuery(function ($) {
        "use strict";

    /* ===================================
       Multipage Side Menu
       ====================================== */

    if ($("#sidemenu_toggle").length) {

            /* Multipage SideNav */

            /* Multi Items Main Menu */

            $(".multi-item1, .multi-item2, .multi-item3, .multi-item4, .multi-item5, .multi-item6, .multi-item7, .multi-item8, .multi-item9, .multi-item10").on("click", function () {
                $(".side-main-menu").addClass("toggle"),$(".side-menu").addClass("side-menu-active"), $("#close_side_menu").fadeOut(200), $(".pushwrap").removeClass("active")
            }),

            $(".multi-item1").on("click", function () {$(".sub-multi-item1").addClass("toggle")}),
            $(".multi-item2").on("click", function () {$(".sub-multi-item2").addClass("toggle")}),
            $(".multi-item3").on("click", function () {$(".sub-multi-item3").addClass("toggle")}),
            $(".multi-item4").on("click", function () {$(".sub-multi-item4").addClass("toggle")}),
            $(".multi-item5").on("click", function () {$(".sub-multi-item5").addClass("toggle")}),
            $(".multi-item6").on("click", function () {$(".sub-multi-item6").addClass("toggle")}),
            $(".multi-item7").on("click", function () {$(".sub-multi-item7").addClass("toggle")}),
            $(".multi-item8").on("click", function () {$(".sub-multi-item8").addClass("toggle")}),
            $(".multi-item9").on("click", function () {$(".sub-multi-item9").addClass("toggle")}),
            $(".multi-item10").on("click", function () {$(".sub-multi-item10").addClass("toggle")}),

            /* Multi Items 1 */

            $(".sub-multi-item1 .item1, .sub-multi-item1 .item2, .sub-multi-item1 .item3, .sub-multi-item1 .item4, .sub-multi-item1 .item5, .sub-multi-item1 .item6, .sub-multi-item1 .item7, .sub-multi-item1 .item8, .sub-multi-item1 .item9, .sub-multi-item1 .item10").on("click", function () {
                $(".sub-multi-item1").addClass("toggle-inner"), $(".side-menu").addClass("side-menu-active"), $("#close_side_menu").fadeOut(200), $(".pushwrap").removeClass("active")
            }),

            $(".sub-multi-item1 .item1").on("click", function () {$(".inner-multi-item1.item1").addClass("toggle")}),
            $(".sub-multi-item1 .item2").on("click", function () {$(".inner-multi-item1.item2").addClass("toggle")}),
            $(".sub-multi-item1 .item3").on("click", function () {$(".inner-multi-item1.item3").addClass("toggle")}),
            $(".sub-multi-item1 .item4").on("click", function () {$(".inner-multi-item1.item4").addClass("toggle")}),
            $(".sub-multi-item1 .item5").on("click", function () {$(".inner-multi-item1.item5").addClass("toggle")}),
            $(".sub-multi-item1 .item6").on("click", function () {$(".inner-multi-item1.item6").addClass("toggle")}),
            $(".sub-multi-item1 .item7").on("click", function () {$(".inner-multi-item1.item7").addClass("toggle")}),
            $(".sub-multi-item1 .item8").on("click", function () {$(".inner-multi-item1.item8").addClass("toggle")}),
            $(".sub-multi-item1 .item9").on("click", function () {$(".inner-multi-item1.item9").addClass("toggle")}),
            $(".sub-multi-item1 .item10").on("click", function () {$(".inner-multi-item1.item10").addClass("toggle")}),

            /* Multi Items 2 */

            $(".sub-multi-item2 .item1, .sub-multi-item2 .item2, .sub-multi-item2 .item3, .sub-multi-item2 .item4, .sub-multi-item2 .item5, .sub-multi-item2 .item6, .sub-multi-item2 .item7, .sub-multi-item2 .item8, .sub-multi-item2 .item9, .sub-multi-item2 .item10").on("click", function () {
                $(".sub-multi-item2").addClass("toggle-inner"), $(".side-menu").addClass("side-menu-active"), $("#close_side_menu").fadeOut(200), $(".pushwrap").removeClass("active")
            }),

            $(".sub-multi-item2 .item1").on("click", function () {$(".inner-multi-item2.item1").addClass("toggle")}),
            $(".sub-multi-item2 .item2").on("click", function () {$(".inner-multi-item2.item2").addClass("toggle")}),
            $(".sub-multi-item2 .item3").on("click", function () {$(".inner-multi-item2.item3").addClass("toggle")}),
            $(".sub-multi-item2 .item4").on("click", function () {$(".inner-multi-item2.item4").addClass("toggle")}),
            $(".sub-multi-item2 .item5").on("click", function () {$(".inner-multi-item2.item5").addClass("toggle")}),
            $(".sub-multi-item2 .item6").on("click", function () {$(".inner-multi-item2.item6").addClass("toggle")}),
            $(".sub-multi-item2 .item7").on("click", function () {$(".inner-multi-item2.item7").addClass("toggle")}),
            $(".sub-multi-item2 .item8").on("click", function () {$(".inner-multi-item2.item8").addClass("toggle")}),
            $(".sub-multi-item2 .item9").on("click", function () {$(".inner-multi-item2.item9").addClass("toggle")}),
            $(".sub-multi-item2 .item10").on("click", function () {$(".inner-multi-item2.item10").addClass("toggle")}),

            /* Multi Items 3 */

            $(".sub-multi-item3 .item1, .sub-multi-item3 .item2, .sub-multi-item3 .item3, .sub-multi-item3 .item4, .sub-multi-item3 .item5, .sub-multi-item3 .item6, .sub-multi-item3 .item7, .sub-multi-item3 .item8, .sub-multi-item3 .item9, .sub-multi-item3 .item10").on("click", function () {
                $(".sub-multi-item3").addClass("toggle-inner"), $(".side-menu").addClass("side-menu-active"), $("#close_side_menu").fadeOut(200), $(".pushwrap").removeClass("active")
            }),

            $(".sub-multi-item3 .item1").on("click", function () {$(".inner-multi-item3.item1").addClass("toggle")}),
            $(".sub-multi-item3 .item2").on("click", function () {$(".inner-multi-item3.item2").addClass("toggle")}),
            $(".sub-multi-item3 .item3").on("click", function () {$(".inner-multi-item3.item3").addClass("toggle")}),
            $(".sub-multi-item3 .item4").on("click", function () {$(".inner-multi-item3.item4").addClass("toggle")}),
            $(".sub-multi-item3 .item5").on("click", function () {$(".inner-multi-item3.item5").addClass("toggle")}),
            $(".sub-multi-item3 .item6").on("click", function () {$(".inner-multi-item3.item6").addClass("toggle")}),
            $(".sub-multi-item3 .item7").on("click", function () {$(".inner-multi-item3.item7").addClass("toggle")}),
            $(".sub-multi-item3 .item8").on("click", function () {$(".inner-multi-item3.item8").addClass("toggle")}),
            $(".sub-multi-item3 .item9").on("click", function () {$(".inner-multi-item3.item9").addClass("toggle")}),
            $(".sub-multi-item3 .item10").on("click", function () {$(".inner-multi-item3.item10").addClass("toggle")}),

            /* Multi Items 4 */

            $(".sub-multi-item4 .item1, .sub-multi-item4 .item2, .sub-multi-item4 .item3, .sub-multi-item4 .item4, .sub-multi-item4 .item5, .sub-multi-item4 .item6, .sub-multi-item4 .item7, .sub-multi-item4 .item8, .sub-multi-item4 .item9, .sub-multi-item4 .item10").on("click", function () {
                $(".sub-multi-item4").addClass("toggle-inner"), $(".side-menu").addClass("side-menu-active"), $("#close_side_menu").fadeOut(200), $(".pushwrap").removeClass("active")
            }),

            $(".sub-multi-item4 .item1").on("click", function () {$(".inner-multi-item4.item1").addClass("toggle")}),
            $(".sub-multi-item4 .item2").on("click", function () {$(".inner-multi-item4.item2").addClass("toggle")}),
            $(".sub-multi-item4 .item3").on("click", function () {$(".inner-multi-item4.item3").addClass("toggle")}),
            $(".sub-multi-item4 .item4").on("click", function () {$(".inner-multi-item4.item4").addClass("toggle")}),
            $(".sub-multi-item4 .item5").on("click", function () {$(".inner-multi-item4.item5").addClass("toggle")}),
            $(".sub-multi-item4 .item6").on("click", function () {$(".inner-multi-item4.item6").addClass("toggle")}),
            $(".sub-multi-item4 .item7").on("click", function () {$(".inner-multi-item4.item7").addClass("toggle")}),
            $(".sub-multi-item4 .item8").on("click", function () {$(".inner-multi-item4.item8").addClass("toggle")}),
            $(".sub-multi-item4 .item9").on("click", function () {$(".inner-multi-item4.item9").addClass("toggle")}),
            $(".sub-multi-item4 .item10").on("click", function () {$(".inner-multi-item4.item10").addClass("toggle")}),

            /* Multi Items 5 */

            $(".sub-multi-item5 .item1, .sub-multi-item5 .item2, .sub-multi-item5 .item3, .sub-multi-item5 .item4, .sub-multi-item5 .item5, .sub-multi-item5 .item6, .sub-multi-item5 .item7, .sub-multi-item5 .item8, .sub-multi-item5 .item9, .sub-multi-item5 .item10").on("click", function () {
                $(".sub-multi-item5").addClass("toggle-inner"), $(".side-menu").addClass("side-menu-active"), $("#close_side_menu").fadeOut(200), $(".pushwrap").removeClass("active")
            }),

            $(".sub-multi-item5 .item1").on("click", function () {$(".inner-multi-item5.item1").addClass("toggle")}),
            $(".sub-multi-item5 .item2").on("click", function () {$(".inner-multi-item5.item2").addClass("toggle")}),
            $(".sub-multi-item5 .item3").on("click", function () {$(".inner-multi-item5.item3").addClass("toggle")}),
            $(".sub-multi-item5 .item4").on("click", function () {$(".inner-multi-item5.item4").addClass("toggle")}),
            $(".sub-multi-item5 .item5").on("click", function () {$(".inner-multi-item5.item5").addClass("toggle")}),
            $(".sub-multi-item5 .item6").on("click", function () {$(".inner-multi-item5.item6").addClass("toggle")}),
            $(".sub-multi-item5 .item7").on("click", function () {$(".inner-multi-item5.item7").addClass("toggle")}),
            $(".sub-multi-item5 .item8").on("click", function () {$(".inner-multi-item5.item8").addClass("toggle")}),
            $(".sub-multi-item5 .item9").on("click", function () {$(".inner-multi-item5.item9").addClass("toggle")}),
            $(".sub-multi-item5 .item10").on("click", function () {$(".inner-multi-item5.item10").addClass("toggle")}),

                /* Multi Items 6 */

                $(".sub-multi-item6 .item1, .sub-multi-item6 .item2, .sub-multi-item6 .item3, .sub-multi-item6 .item4, .sub-multi-item6 .item5, .sub-multi-item6 .item6, .sub-multi-item6 .item7, .sub-multi-item6 .item8, .sub-multi-item6 .item9, .sub-multi-item6 .item10").on("click", function () {
                    $(".sub-multi-item6").addClass("toggle-inner"), $(".side-menu").addClass("side-menu-active"), $("#close_side_menu").fadeOut(200), $(".pushwrap").removeClass("active")
                }),

                $(".sub-multi-item6 .item1").on("click", function () {$(".inner-multi-item6.item1").addClass("toggle")}),
                $(".sub-multi-item6 .item2").on("click", function () {$(".inner-multi-item6.item2").addClass("toggle")}),
                $(".sub-multi-item6 .item3").on("click", function () {$(".inner-multi-item6.item3").addClass("toggle")}),
                $(".sub-multi-item6 .item4").on("click", function () {$(".inner-multi-item6.item4").addClass("toggle")}),
                $(".sub-multi-item6 .item5").on("click", function () {$(".inner-multi-item6.item5").addClass("toggle")}),
                $(".sub-multi-item6 .item6").on("click", function () {$(".inner-multi-item6.item6").addClass("toggle")}),
                $(".sub-multi-item6 .item7").on("click", function () {$(".inner-multi-item6.item7").addClass("toggle")}),
                $(".sub-multi-item6 .item8").on("click", function () {$(".inner-multi-item6.item8").addClass("toggle")}),
                $(".sub-multi-item6 .item9").on("click", function () {$(".inner-multi-item6.item9").addClass("toggle")}),
                $(".sub-multi-item6 .item10").on("click", function () {$(".inner-multi-item6.item10").addClass("toggle")}),

                /* Multi Items 7 */

                $(".sub-multi-item7 .item1, .sub-multi-item7 .item2, .sub-multi-item7 .item3, .sub-multi-item7 .item4, .sub-multi-item7 .item5, .sub-multi-item7 .item6, .sub-multi-item7 .item7, .sub-multi-item7 .item8, .sub-multi-item7 .item9, .sub-multi-item7 .item10").on("click", function () {
                    $(".sub-multi-item7").addClass("toggle-inner"), $(".side-menu").addClass("side-menu-active"), $("#close_side_menu").fadeOut(200), $(".pushwrap").removeClass("active")
                }),

                $(".sub-multi-item7 .item1").on("click", function () {$(".inner-multi-item7.item1").addClass("toggle")}),
                $(".sub-multi-item7 .item2").on("click", function () {$(".inner-multi-item7.item2").addClass("toggle")}),
                $(".sub-multi-item7 .item3").on("click", function () {$(".inner-multi-item7.item3").addClass("toggle")}),
                $(".sub-multi-item7 .item4").on("click", function () {$(".inner-multi-item7.item4").addClass("toggle")}),
                $(".sub-multi-item7 .item5").on("click", function () {$(".inner-multi-item7.item5").addClass("toggle")}),
                $(".sub-multi-item7 .item6").on("click", function () {$(".inner-multi-item7.item6").addClass("toggle")}),
                $(".sub-multi-item7 .item7").on("click", function () {$(".inner-multi-item7.item7").addClass("toggle")}),
                $(".sub-multi-item7 .item8").on("click", function () {$(".inner-multi-item7.item8").addClass("toggle")}),
                $(".sub-multi-item7 .item9").on("click", function () {$(".inner-multi-item7.item9").addClass("toggle")}),
                $(".sub-multi-item7 .item10").on("click", function () {$(".inner-multi-item7.item10").addClass("toggle")}),

                /* Multi Items 8 */

                $(".sub-multi-item8 .item1, .sub-multi-item8 .item2, .sub-multi-item8 .item3, .sub-multi-item8 .item4, .sub-multi-item8 .item5, .sub-multi-item8 .item6, .sub-multi-item8 .item7, .sub-multi-item8 .item8, .sub-multi-item8 .item9, .sub-multi-item8 .item10").on("click", function () {
                    $(".sub-multi-item8").addClass("toggle-inner"), $(".side-menu").addClass("side-menu-active"), $("#close_side_menu").fadeOut(200), $(".pushwrap").removeClass("active")
                }),

                $(".sub-multi-item8 .item1").on("click", function () {$(".inner-multi-item8.item1").addClass("toggle")}),
                $(".sub-multi-item8 .item2").on("click", function () {$(".inner-multi-item8.item2").addClass("toggle")}),
                $(".sub-multi-item8 .item3").on("click", function () {$(".inner-multi-item8.item3").addClass("toggle")}),
                $(".sub-multi-item8 .item4").on("click", function () {$(".inner-multi-item8.item4").addClass("toggle")}),
                $(".sub-multi-item8 .item5").on("click", function () {$(".inner-multi-item8.item5").addClass("toggle")}),
                $(".sub-multi-item8 .item6").on("click", function () {$(".inner-multi-item8.item6").addClass("toggle")}),
                $(".sub-multi-item8 .item7").on("click", function () {$(".inner-multi-item8.item7").addClass("toggle")}),
                $(".sub-multi-item8 .item8").on("click", function () {$(".inner-multi-item8.item8").addClass("toggle")}),
                $(".sub-multi-item8 .item9").on("click", function () {$(".inner-multi-item8.item9").addClass("toggle")}),
                $(".sub-multi-item8 .item10").on("click", function () {$(".inner-multi-item8.item10").addClass("toggle")}),

                /* Multi Items 9 */

                $(".sub-multi-item9 .item1, .sub-multi-item9 .item2, .sub-multi-item9 .item3, .sub-multi-item9 .item4, .sub-multi-item9 .item5, .sub-multi-item9 .item6, .sub-multi-item9 .item7, .sub-multi-item9 .item8, .sub-multi-item9 .item9, .sub-multi-item9 .item10").on("click", function () {
                    $(".sub-multi-item9").addClass("toggle-inner"), $(".side-menu").addClass("side-menu-active"), $("#close_side_menu").fadeOut(200), $(".pushwrap").removeClass("active")
                }),

                $(".sub-multi-item9 .item1").on("click", function () {$(".inner-multi-item9.item1").addClass("toggle")}),
            $(".sub-multi-item9 .item2").on("click", function () {$(".inner-multi-item9.item2").addClass("toggle")}),
            $(".sub-multi-item9 .item3").on("click", function () {$(".inner-multi-item9.item3").addClass("toggle")}),
            $(".sub-multi-item9 .item4").on("click", function () {$(".inner-multi-item9.item4").addClass("toggle")}),
            $(".sub-multi-item9 .item5").on("click", function () {$(".inner-multi-item9.item5").addClass("toggle")}),
            $(".sub-multi-item9 .item6").on("click", function () {$(".inner-multi-item9.item6").addClass("toggle")}),
            $(".sub-multi-item9 .item7").on("click", function () {$(".inner-multi-item9.item7").addClass("toggle")}),
            $(".sub-multi-item9 .item8").on("click", function () {$(".inner-multi-item9.item8").addClass("toggle")}),
            $(".sub-multi-item9 .item9").on("click", function () {$(".inner-multi-item9.item9").addClass("toggle")}),
            $(".sub-multi-item9 .item10").on("click", function () {$(".inner-multi-item9.item10").addClass("toggle")}),

            /* Multi Items 10 */

            $(".sub-multi-item10 .item1, .sub-multi-item10 .item2, .sub-multi-item10 .item3, .sub-multi-item10 .item4, .sub-multi-item10 .item5, .sub-multi-item10 .item6, .sub-multi-item10 .item7, .sub-multi-item10 .item8, .sub-multi-item10 .item9, .sub-multi-item10 .item10").on("click", function () {
                $(".sub-multi-item10").addClass("toggle-inner"), $(".side-menu").addClass("side-menu-active"), $("#close_side_menu").fadeOut(200), $(".pushwrap").removeClass("active")
            }),

            $(".sub-multi-item10 .item1").on("click", function () {$(".inner-multi-item10.item1").addClass("toggle")}),
            $(".sub-multi-item10 .item2").on("click", function () {$(".inner-multi-item10.item2").addClass("toggle")}),
            $(".sub-multi-item10 .item3").on("click", function () {$(".inner-multi-item10.item3").addClass("toggle")}),
            $(".sub-multi-item10 .item4").on("click", function () {$(".inner-multi-item10.item4").addClass("toggle")}),
            $(".sub-multi-item10 .item5").on("click", function () {$(".inner-multi-item10.item5").addClass("toggle")}),
            $(".sub-multi-item10 .item6").on("click", function () {$(".inner-multi-item10.item6").addClass("toggle")}),
            $(".sub-multi-item10 .item7").on("click", function () {$(".inner-multi-item10.item7").addClass("toggle")}),
            $(".sub-multi-item10 .item8").on("click", function () {$(".inner-multi-item10.item8").addClass("toggle")}),
            $(".sub-multi-item10 .item9").on("click", function () {$(".inner-multi-item10.item9").addClass("toggle")}),
            $(".sub-multi-item10 .item10").on("click", function () {$(".inner-multi-item10.item10").addClass("toggle")}),


            /* Single Items */

            $(".side-main-menu .single-item1, .side-main-menu .single-item2, .side-main-menu .single-item3, .side-main-menu .single-item4, .side-main-menu .single-item5, .side-main-menu .single-item6, .side-main-menu .item7, .side-main-menu .single-item8, .side-main-menu .single-item9, .side-main-menu .single-item10").on("click", function () {
                $(".side-main-menu").addClass("toggle"),$(".side-menu").addClass("side-menu-active"), $("#close_side_menu").fadeOut(200), $(".pushwrap").removeClass("active")
            }),

            $(".side-main-menu .single-item1").on("click", function () {$(".side-sub-menu.single-item1").addClass("toggle")}),
            $(".side-main-menu .single-item2").on("click", function () {$(".side-sub-menu.single-item2").addClass("toggle")}),
            $(".side-main-menu .single-item3").on("click", function () {$(".side-sub-menu.single-item3").addClass("toggle")}),
            $(".side-main-menu .single-item4").on("click", function () {$(".side-sub-menu.single-item4").addClass("toggle")}),
            $(".side-main-menu .single-item5").on("click", function () {$(".side-sub-menu.single-item5").addClass("toggle")}),
            $(".side-main-menu .single-item6").on("click", function () {$(".side-sub-menu.single-item6").addClass("toggle")}),
            $(".side-main-menu .single-item7").on("click", function () {$(".side-sub-menu.single-item7").addClass("toggle")}),
            $(".side-main-menu .single-item8").on("click", function () {$(".side-sub-menu.single-item8").addClass("toggle")}),
            $(".side-main-menu .single-item9").on("click", function () {$(".side-sub-menu.single-item9").addClass("toggle")}),
            $(".side-main-menu .single-item10").on("click", function () {$(".side-sub-menu.single-item10").addClass("toggle")}),

            /* Back To Main Button Toggle */

            $(".back-main").on("click", function () {
                $(".side-main-menu, .side-sub-menu, .sub-multi-item1, .sub-multi-item2, .sub-multi-item3, .sub-multi-item4, .sub-multi-item5, .sub-multi-item6, .sub-multi-item7, .sub-multi-item8, .sub-multi-item9, .sub-multi-item10, .inner-multi-item1, .inner-multi-item2, .inner-multi-item3, .inner-multi-item4, .inner-multi-item5, .inner-multi-item6, .inner-multi-item7, .inner-multi-item8, .inner-multi-item9, .inner-multi-item10").removeClass("toggle"),
                    $(".sub-multi-item1, .sub-multi-item2, .sub-multi-item3, .sub-multi-item4, .sub-multi-item5, .sub-multi-item6, .sub-multi-item7, .sub-multi-item8, .sub-multi-item9, .sub-multi-item10").removeClass("toggle-inner"),
                    $(".side-menu").addClass("side-menu-active"),
                    $("#close_side_menu").fadeOut(200), $(".pushwrap").removeClass("active")
            }),

            /* Back Button Toggle */

            $(".back").on("click", function () {
                $(".inner-multi-item1, .inner-multi-item2, .inner-multi-item3, .inner-multi-item4, .inner-multi-item5, .inner-multi-item6, .inner-multi-item7, .inner-multi-item8, .inner-multi-item9, .inner-multi-item10").removeClass("toggle"),
                    $(".sub-multi-item1, .sub-multi-item2, .sub-multi-item3, .sub-multi-item4, .sub-multi-item5, .sub-multi-item6, .sub-multi-item7, .sub-multi-item8, .sub-multi-item9, .sub-multi-item10").removeClass("toggle-inner"),
                    $(".side-menu").addClass("side-menu-active"), $("#close_side_menu").fadeOut(200), $(".pushwrap").removeClass("active")
            });
    }

    });
});