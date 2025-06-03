import { defineStore } from 'pinia'
import { onMounted, ref } from 'vue'
import { getAppNameApi } from '../apis/app'

export const useTitleStore = defineStore('title', () => {
  const title = ref('')
  const loading = ref(false)

  const updateDocumentTitle = () => {
    if (title.value) {
      window.document.title = title.value
    } else {
      window.document.title = 'FA OEM App'
    }
  }

  const setTitle = (value) => {
    title.value = value
    updateDocumentTitle()
  }

  const getAppName = async () => {
    if (loading.value) {
      return
    }

    loading.value = true
    try {
      const res = await getAppNameApi()
      const { value } = res.data
      title.value = value
      updateDocumentTitle()
    } finally {
      loading.value = false
    }
  }

  onMounted(async () => {
    await getAppName()
  })

  return { title, setTitle }
})
