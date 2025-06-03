import { onMounted, ref } from 'vue'
import { getDefinitionsApi } from '../apis/variableMonitor'

export const useGetDefinitions = () => {
  const loading = ref(false)
  const data = ref({
    total: 0,
    items: [],
  })
  const pageIndex = ref(1)
  const pageSize = ref(20)
  const nameFilter = ref('')

  const getDefinitions = async () => {
    if (loading.value) {
      return
    }

    loading.value = true

    try {
      const res = await getDefinitionsApi({
        name: nameFilter.value,
        pageIndex: pageIndex.value,
        pageSize: pageSize.value,
      })
      data.value = res.data
    } catch (e) {
      console.error(e)
    } finally {
      loading.value = false
    }
  }

  onMounted(async () => {
    await getDefinitions()
  })

  const handlePageChange = async () => {
    await getDefinitions()
  }

  const handlePageSizeChange = async () => {
    await getDefinitions()
  }

  return {
    loading,
    data,
    pageIndex,
    pageSize,
    nameFilter,
    getDefinitions,
    handlePageChange,
    handlePageSizeChange,
  }
}
