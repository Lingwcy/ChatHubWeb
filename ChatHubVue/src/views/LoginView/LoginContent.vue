<script lang="ts" setup>
document.body.style.overflow = "hidden"



</script>
<template>
    <div id="Logincontainer">
        <el-col :xs="24" :sm="24" :md="0" :lg="0" :xl="0" class="Logincontainer-Phone hidden-md-and-up">
            <div id="title-phone1">欢迎回来!</div>
            <div id="title-phone2">很高兴再次见到您！</div>
            <router-view></router-view>
        </el-col>
        <el-col :xs="0" :sm="0" :md="24" :lg="24" :xl="24" class="Logincontainer-Window hidden-sm-and-down">
            <div id="LoginContentContainer">
            <div id="leftScan">
                <div class="leftScan-content">
                    <div class="leftScan-img">
                        <img class="Qrcode-qrcode" src="../../images/scancode.png" alt="二维码" width="150" height="150">
                    </div>
                    <div class="leftScan-font1">通过二维码登录</div>
                    <div class="leftScan-font2">通过「零时通讯」手机APP扫描二维码，便可即时登录。</div>
                </div>
            </div>
            <div id="rightLogin">
                <div id="rightLogin-header">
                    <div id="rightLogin-header-title1">欢迎回来</div>
                    <div id="rightLogin-header-title2">很高兴再次见到您！</div>
                </div>
                <div id="rightLogin-content">
                    <div id="rightLogin-content-main">
                        <div id="rightLogin-content-main-center">
                            <router-view></router-view>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        </el-col>
    </div>
</template>
<style>
#title-phone1{
    margin-top: 250px;
    font-size: 25px;
    margin-bottom: 10px;
    font-weight: 600;
}
#title-phone2{
    font-size: 16px;
    margin-bottom: 20px;
    color: whitesmoke;
    color: #a7a7a8
}

#Logincontainer{
    width: 100%;
    height: 100%;
}
.Logincontainer-Phone{
    width: 100%;
    height: 100%;
    align-items: center;
    display: flex;
    flex-direction: column;
    justify-content: center;

}
.Logincontainer-Window{
    width: 100%;
    height: 100%;
    align-items: center;
    display: flex;
    justify-content: center;
}
#rightLogin-content-footer {
    width: 50%;
    height: 15%;
    margin-left: 130px;
    margin-top: 10px;
    display: flex;
    justify-content: space-between;
}

.rightLogin-content-footer-other {
    width: 35px;
    height: 35px;
    padding: 0;
    border-radius: 30px;
    background-color: rgb(225, 226, 228);

}

.leftScan-font1 {
    margin-top: 5px;
    font-size: 25px;
    margin-bottom: 10px;
    font-weight: 600;
}

.leftScan-font2 {
    font-size: 15px;
    margin-bottom: 20px;
    color: whitesmoke;
    color: #a7a7a8
}

.leftScan-img {
    margin-bottom: 20px;
    width: 180px;
    height: 180px;
    border-radius: 5px;
    border: 1.5px solid rgb(187, 185, 185);
    display: flex;
    justify-content: center;
    align-items: center;
}

.Qrcode-qrcode {
    width: 170px;
    height: 170px;
}

.QleftScan-message-font {
    font-size: 15px;
    margin-top: 30px;
    color: whitesmoke;
}
.el-input:hover {
    --el-input-hover-border-color: null;
}

.el-input__wrapper {
    box-shadow: none;
    border-radius: 0px;
    transition: border-bottom 0.3s ease-in-out;
    background-color: unset;
}

.el-input__wrapper.is-focus {
    box-shadow: none;
    border-bottom: 2px solid #409eff;

}

#LoginContentContainer {
    width: 800px;
    height: 400px;
    flex-direction: row-reverse;
    margin-bottom: 100px;
    display: flex;
    box-shadow: 2px 2px 2px 1px rgba(0, 0, 0, 0.2);
}

#leftScan {
    width: 40%;
    height: 100%;
    box-sizing: border-box;
    display: flex;
    justify-content: center;
    align-items: center;
    flex-direction: column;


}

.leftScan-content {
    box-sizing: border-box;
    display: flex;
    justify-content: center;
    align-items: center;
    flex-direction: column;
    width: 80%;
}

#rightLogin {
    border-radius: 10px;
    width: 60%;
}

#rightLogin-header {
    width: 100%;
    display: flex;
    justify-content: center;
}

#rightLogin-header-container {
    width: 100%;
}

.rightLogin-header-container-items {
    font-size: 18px;
}

.flex-grow {
    flex-grow: 1;
}

#loginNavBar {
    border-bottom: none;
}

#rightLogin-content {
    width: 100%;
    height: 80%;
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
}

#rightLogin-header {
    margin-top: 40PX;
    display: flex;
    justify-content: center;
    align-items: center;
    flex-direction: column;
}

#rightLogin-header-title1 {
    margin: 10px;
    font-size: 27px;
    font-weight: bolder;
}

#rightLogin-header-title2 {
    font-size: 16px;
}

#rightLogin-content-main {
    width: 100%;
    height: 90%;
    display: flex;
    justify-content: center;
    align-items: center;
    min-width: 450px;
}

#rightLogin-content-main-center {
    display: flex;
    flex-direction: column;
    height: 100%;
    width: 100%;
}

.loginBt {
    width: 350px;
}</style>
