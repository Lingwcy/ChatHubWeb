<template>
    <div class="emojiTools">
       <div style="height: 20px; font-size: 12px; display: flex; justify-content: end; align-items: center; margin-bottom: 5px;">
            <button id="emojiSetting">设置</button>
       </div> 
      <el-tabs :tab-position="tabPosition" style="height: 140px" class="emoji-tabs">
      <el-tab-pane label="收藏">

      </el-tab-pane>
      <el-tab-pane label="经典">
        <span class="emojiItems" v-for="(item) in classicEmojiList"  v-on:click="appset.input+=item">{{ item }}</span>
      </el-tab-pane>
      <el-tab-pane label="原生">WCY</el-tab-pane>
    </el-tabs>
    </div>

  </template>
  <script lang="ts" setup>
  import { ref } from 'vue'
  import { appsetting } from '../../../../store/index'
  const appset =appsetting() 
  
  const tabPosition = ref('bottom')


  let emojiStr ="😄,😆,😊,😃,😏,😍,😘,😚,😳,😌,😆,😁,😉,😜,😝,😀,😗,😙,😛,😴,😟,😦,😧,😮,😬,😕,😯,😑,😒,😅,😓,😥,😩,😔,😞,😖,😨,😰,😣,😢,😭,😂,😲,😱,😫,😠,😡,😤,😪,😋,😷,😎,😵,👿,😐,😶,😇,👽,💛,💙,💜,❤️,💚,💔,💓,💗,💕,💖,✨,⭐,🌟,💫,💥,💢,❗,❓,❕,❔,💤,💨,💦,🎶,🎵,🔥,💩,👍,👎,👌,👊,✊,👋,👐,☝️,👇,👈,👉,🙌,🙏,👆,👏,💪,🤘,🖕,🏃,👫,👪,👭"

  const classicEmojiList=emojiStr.split(",")
  </script>
  <style>
  .emoji-tabs > .el-tabs__content {
    height: 100px;
    display: flex;
    flex-direction: row;
    width: 480px;
    align-items: center;
  }
  .emoji-tabs > .el-tabs__header {
    margin-top: 0 !important;
  }
  .emojiItems:hover{
    transition: 0.3s all ease-in-out;
    background-color: rgb(211, 210, 210);
    cursor: pointer;
    border-radius: 10px;
  }
  #emojiSetting{
    font-size: 10px !important;
    margin-right: 10px;
    color: rgb(100, 97, 97);
    font-weight: 300;
  }
  </style>
  