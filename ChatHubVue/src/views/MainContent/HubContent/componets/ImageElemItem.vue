<template>
  <div
    class="image_preview select-none"
    :class="self ? 'is-text-self' : 'is-text-other'"
  >
    <el-image
      :src="message.src"
      :hide-on-click-modal="true"
      :infinite="false"
      :preview-src-list="[message.src]"
      fit="cover"
    />

    
  </div>
</template>

<script lang="ts" setup>

const props = defineProps({
  message: {
    type: Object,
    default: null,
  },
  self: {
    type: Boolean,
    default: false,
  },
});

</script>

<style lang="scss" scoped>
.is-text-self {
  background: var(--self-msg-color);
}
.is-text-other {
  background: var(--other-msg-color);
}
.image_preview {
  width: fit-content;
  max-width: 350px;
  // padding: 10px 14px;
  box-sizing: border-box;
  border-radius: 5px;
  border: 1px solid rgb(208, 221, 215);
  :deep(.el-image) {
    border-radius: 5px;
    vertical-align: bottom;
  }
}
</style>
