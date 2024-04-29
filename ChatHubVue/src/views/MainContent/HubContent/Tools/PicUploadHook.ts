import { ref } from 'vue'
import type {UploadInstance } from 'element-plus'
const uploadRef = ref<UploadInstance>()
const submitUpload = () => {
    uploadRef.value!.submit()
  }

export {
    uploadRef,
    submitUpload,
}