import { defineStore } from 'pinia'
import { ref } from 'vue'

export const useAuthenticationStore = defineStore('authentication', () => {
  const showMsg = ref(true)

  const setShowMsg = (show = true) => {
    showMsg.value = show
  }

  return { showMsg, setShowMsg }
})
