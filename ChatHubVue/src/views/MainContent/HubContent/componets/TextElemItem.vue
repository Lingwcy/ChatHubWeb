<template>
  <div
    class="message-view__item--text"
    :class="self ? 'is-text-self' : 'is-text-other'"
    
  >
    <span>
      <DynamicContent islink :text="message" />
    </span>
  </div>
</template>

<script lang="ts" setup>
import DynamicContent from "./DynamicContent.vue";
const props = defineProps({
  msgType: {
    type: String,
    default: "",
  },
  message: {
    type: String,
    default: "",
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
.emoji {
  width: 23px;
  vertical-align: sub;
}
.message-view__item--text {
  width: fit-content;
  padding: 10px 14px;
  // max-width: 360px;
  box-sizing: border-box;
  border-radius: 3px;
  word-break: break-all;
  white-space: pre-wrap;
  color: var(--color-text);
  ::selection {
    background-color: rgb(193, 203, 244);
  }
}
</style>
