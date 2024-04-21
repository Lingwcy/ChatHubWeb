<template>
  <el-dialog v-model="appset.CompoentsEvent.isSlideVerify.isOpen" title="登录验证" width="500" :before-close="handleClose">
    <div class="silde_box">
      <slide-verify class="silde_box" ref="block" :slider-text="text" :accuracy="accuracy" @again="onAgain"
        @success="onSuccess" @fail="onFail" @refresh="onRefresh"></slide-verify>
    </div>
  </el-dialog>
</template>

<script lang="ts" setup>
import { appsetting } from '../../store';
import { ref } from 'vue';
import SlideVerify, { SlideVerifyInstance } from 'vue3-slide-verify';
import 'vue3-slide-verify/dist/style.css';
import { ElMessage, ElMessageBox } from 'element-plus'
const appset = appsetting();

const handleClose = (done: () => void) => {
  ElMessageBox.confirm('确认要取消登录吗？')
    .then(() => {
      done()
    })
    .catch(() => {
      // catch error
    })
}
const msg = ref('');
const block = ref<SlideVerifyInstance | null>(null);

const text = '向右滑动->';
const accuracy = 1;

const onAgain = () => {
  ElMessage({
    message: '验证出现错误，请重新验证.',
    type: 'warning',
  })

  // 刷新  
  block.value?.refresh();
};

const onSuccess = (times: number) => {
  appset.CompoentsEvent.isSlideVerify.isSuccess = true;
  block.value?.refresh();

};

const onFail = () => {
  ElMessage({
    message: '验证不通过，请重试.',
    type: 'warning',
  })

};

const onRefresh = () => {
  msg.value = '点击了刷新小图标';
};
 
</script>

<style scoped>
.silde_box {
  margin: 0 auto;
}
</style>