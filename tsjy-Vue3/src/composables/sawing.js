import { ref } from 'vue'
import { getRequestAllVariableApi } from '@/apis/sawing.js'

export const useSawing = () => {
  const loadingRequestAllVariable = ref(false)
  const requestAllVariable = async () => {
    if (loadingRequestAllVariable.value) {
      return
    }
    loadingRequestAllVariable.value = true
    try {
      await getRequestAllVariableApi()
    } catch (e) {
      console.error(e)
    } finally {
      loadingRequestAllVariable.value = false
    }
  }
  return { requestAllVariable }
}
