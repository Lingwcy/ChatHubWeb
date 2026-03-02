
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

// 全局 Vue 错误处理
Mains.config.errorHandler = (err, instance, info) => {
    console.error('Vue Error:', err);
    // 使用 Element Plus 的 ElMessage 需要在组件内，这里使用原生 alert
    alert('发生错误，请刷新页面重试');
};

// 全局 Promise rejection 处理
window.addEventListener('unhandledrejection', (event) => {
    console.error('Unhandled Promise Rejection:', event.reason);
    alert('网络请求失败，请稍后重试');
});

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


// 页面离开时的事件监听（保留原有逻辑）
const beforeUnloadHandler = () => {
    // 可以在这里添加未保存数据的检查
};

window.addEventListener('beforeunload', beforeUnloadHandler);

// load 事件监听
const loadHandler = () => {
    // 页面加载完成后的处理
};
window.addEventListener('load', loadHandler);
