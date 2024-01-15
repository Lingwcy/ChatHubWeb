<script setup lang="ts">

document.body.style.overflow="hidden"
</script>
<template>
    <div id="about-header-content">
        <div id="about-header-content-hero">
            <h1 class="hero-line">
                <span>Chat-Hub</span>
                <br>
                 在线即时聊天集线器
            </h1>
            <p id="hero-detail"> 即时互动，性能出色，适用场景丰富的 在线通讯Web。 </p>
        </div>
        <div id="about-header-content-made">
            <a href="https://cn.vuejs.org/">
                <svg class="logo" viewBox="0 0 128 128" width="35" height="35">
                    <path fill="#42b883" d="M78.8,10L64,35.4L49.2,10H0l64,110l64-110C128,10,78.8,10,78.8,10z"></path>
                    <path fill="#35495e" d="M78.8,10L64,35.4L49.2,10H25.6L64,76l38.4-66H78.8z"></path>
                </svg>
                <span id="about-header-content-made-text">Vue.js</span>
            </a>
            <a href="https://www.microsoft.com/zh-cn/">
                <img src="../../images/net.svg" id="netimg" alt="">
                <span id="about-header-content-made-text">&nbsp;.Net6</span>
            </a>
        </div>
    </div>
</template>
<style>


#about-header-content{
    display: block;
    position: relative;
    width: 100%;
    align-items: center;
    display: flex;
    flex-direction: column;
}
#about-header-content-hero{
    align-items: center;
    margin-top: 70px;
}
.hero-line{
    font-size: 76px;
    line-height:1.25;
    font-weight:900;
    letter-spacing:-1.5px;
    max-width: 960px;
	background: -webkit-linear-gradient(315deg,#5cbd91 15%,#db3846);
	background-clip: text;
	-webkit-background-clip: text;
	-webkit-text-fill-color: transparent;
    margin-bottom: 0px;
}
#hero-detail{

    max-width: 960px;
    line-height: 1.5;
    font-size: 25px;
    margin: 24px auto 40px;
    letter-spacing: 0.2px;
    line-height: 33px;
    margin-top: 24px;
    font-weight: 500;
    text-align: center;
}
#about-header-content-made{
    width: 100%;
    height: 70px;
    border-top: 1px solid rgba(95, 94, 94, 0.29);
    border-bottom: 1px solid rgba(95, 94, 94, 0.29);
    display: flex;
    justify-content: center;
    align-items: center;
}
#about-header-content-made-text{
    font-size: 23px;
}
#about-header-content-made a{
    display: flex;
    justify-content: center;
    align-items: center;
    margin-right: 70px;
    margin-left: 70px;
}
#netimg{
    height: 45px;
    border-radius: 70px;
}
</style>
