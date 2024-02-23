<script setup lang="ts">
import gsap from 'gsap';
document.body.style.overflow = "hidden"
gsap.to(".box",{x:200})
</script>
<template>
    <el-scrollbar height="100vh">
        <div id="MainAbout">
            <div id="FirstCard">
                <div id="CardBG" />
                <div id="about-header-content">
                    <div id="about-header-content-hero">
                        <h1 class="hero-line">
                            <span>Chat-Hub</span>
                            <br>
                            在线即时聊天集线器
                        </h1>
                        <p id="hero-detail"> 即时互动，性能出色，适用场景丰富的 在线通讯Web。 </p>
                    </div>
                </div>
                <button
                    class="GoDown"
                    aria-label="Scroll down">
                    <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24"
                        stroke-width="1.5" stroke="currentColor" aria-hidden="true">
                        <path stroke-linecap="round" stroke-linejoin="round" d="M19.5 13.5L12 21m0 0l-7.5-7.5M12 21V3">
                        </path>
                    </svg>
                </button>
            </div>
            <div class="box"></div>
        </div>
    </el-scrollbar>
</template>
<style scoped>
.box{
    width: 100px; height: 100px; background-color: darkseagreen;
    border-radius: 5px;
}
#MainAbout {
    background-color: black;
    height: 3600px;
    width: 100%;
    display: flex;
    justify-content: flex-start;
    align-items: center;
    flex-direction: column;
}

#FirstCard {
    background-color: var(--arbitrum-orbit-blue);
    width: 92%;
    height: 90vh;
    margin-top: 5px;
    border-radius: 15px;
    position: relative;

}

#CardBG {
    width: 100%;
    height: 100%;
    background-image: url(../../images/assets/grains_ellipse.png);
    background-size: cover;
    opacity: .3;
    mix-blend-mode: overlay;
    position: absolute;
    top: 0;
    left: 0;

}

#about-header-content {
    display: block;
    position: relative;
    width: 40%;
    min-width: 630px;
    align-items: center;
    display: flex;
    flex-direction: column;
    position: absolute;
    left: 0;
    bottom: 0;
    margin-left: -10px;
}

.hero-line {
    font-size: 66px;
    line-height: 1.25;
    font-weight: 900;
    letter-spacing: -1.5px;
    background: -webkit-linear-gradient(315deg, #000f08 15%, #62635d);
    background-clip: text;
    -webkit-background-clip: text;
    -webkit-text-fill-color: transparent;
    margin-bottom: 0px;
}

#hero-detail {
    line-height: 1.5;
    font-size: 24px;
    margin: 24px auto 40px;
    letter-spacing: 0.2px;
    line-height: 33px;
    margin-top: 12px;
    font-weight: 500;
    text-align: center;
}
.GoDown{
    tab-size: 4;
    color-scheme: dark;
    border: 0 solid #e5e7eb;
    margin: 0;
    padding: 0;
    text-transform: none;
    background-color: transparent;
    background-image: none;
    cursor: pointer;
    position: absolute;
    bottom: 20px;
    height: 34px;
    width: 34px;
    border-radius: 9999px;
    border-width: 1px;
    border-color: currentColor;
    right: 20px;
    animation: scroll-down-button-pulse-dark-mode 2s infinite alternate;
}

/* 定义闪烁动画 */  
@keyframes scroll-down-button-pulse-dark-mode {  
    0%, 100% {  
        opacity: 1; /* 按钮完全不透明 */  
        transform: scale(1); /* 按钮不缩放 */  
        background-color: rgb(0, 0, 0); 
    }  
    50% {  
        opacity: 0.5; /* 按钮半透明 */  
        transform: scale(1.1); /* 按钮稍微放大 */ 
        background-color: rgb(32, 32, 32); 
    }  
}
@media screen and (max-width: 660px) {
    #about-header-content {
        min-width: 350px;
        margin-left: 10px;
    }

    .hero-line {
        font-size: 48px;
    }

    #hero-detail {
        font-size: 13px;
        line-height: 10px;
        margin-top: 12px;
    }
}
</style>
