
import { createApp } from 'vue'
import './style.css'
import ElementUI from 'element-plus'
import 'element-plus/theme-chalk/index.css'
import Main from './views/MainContent/Main.vue'
import router from './router/index.ts'
import store from './store/index.ts';
import '../node_modules/element-plus/theme-chalk/display.css'
import contextmenu from "v-contextmenu";
import "v-contextmenu/dist/themes/default.css";
const Token = "";

const Username = "未登录"
const Password = ""




const Mains = createApp(Main)
Mains.use(store)
Mains.use(contextmenu)
Mains.use(ElementUI)
Mains.use(router)
Mains.provide('global', {
	Token,
	Username,
	Password,
	FrinedsList: [],
})
Mains.mount('#MainContent')



$(window).on('load', function () {
	var preloaderFadeOutTime = 500;
	function hidePreloader() {
		var preloader = $('.spinner-wrapper');
		setTimeout(function () {
			preloader.fadeOut(preloaderFadeOutTime);
		}, 500);
	}
	hidePreloader();
});


window.addEventListener('beforeunload', () => {  
	
  })

window.addEventListener('load', () => {  
});