<template>
  <div v-for="item in decodeText(text)" :key="item">
    <span v-if="item.name === 'text'">
      {{ item.text }}
    </span>
  </div>
</template>

<script setup>
import { decodeText } from "../../../../untils/chat/decodeText";
const props = defineProps({
  text: {
    type: String,
    default: "",
  },
  islink: {
    type: Boolean,
    default: false,
  },
});
</script>

<style scoped>
.emoji {
  width: 23px;
  vertical-align: sub;
}
</style>
