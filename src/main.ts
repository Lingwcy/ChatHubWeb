
import { createApp } from 'vue'
import './style.css'
import ElementUI from 'element-plus'
import 'element-plus/theme-chalk/index.css'
import $ from 'jquery'
import Main from './views/MainContent/Main.vue'
import nav from './views/Topbar/nav.vue'
import router from './router/index.ts'
import store from './store/index.ts';
const Token=""
const Username="未登录"
const Password=""


const navs=createApp(nav)
navs.use(ElementUI)
navs.use(store)
navs.use(router)
navs.provide('global',{
    Token,
    Username,
    Password,
})
navs.mount('#nav')

const Mains =createApp(Main)
Mains.use(ElementUI)
Mains.use(router)
Mains.use(store)
Mains.provide('global',{
    Token,
    Username,
    Password,
    FrinedsList:[],
})

Mains.mount('#MainContent')




	$(window).on('load', function() {
		var preloaderFadeOutTime = 500;
		function hidePreloader() {
			var preloader = $('.spinner-wrapper');
			setTimeout(function() {
				preloader.fadeOut(preloaderFadeOutTime);
			}, 500);
		}
		hidePreloader();
	});
