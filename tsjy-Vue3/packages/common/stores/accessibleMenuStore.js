import { defineStore } from 'pinia'
import { onMounted, ref } from 'vue'
import { getMenuApi } from '../apis/roleMenu'

export const useAccessibleMenuStore = defineStore('accessible_menu', () => {
  const accessibleMenu = ref()
  const loading = ref(false)

  const getAccessibleMenu = async () => {
    if (loading.value) {
      return
    }

    loading.value = true

    try {
      const res = await getMenuApi()
      accessibleMenu.value = res.data
    } catch (e) {
      console.error(e)
    } finally {
      loading.value = false
    }
  }

  onMounted(async () => {
    await getAccessibleMenu()
  })

  return {
    accessibleMenu,
    getAccessibleMenu,
  }
})
