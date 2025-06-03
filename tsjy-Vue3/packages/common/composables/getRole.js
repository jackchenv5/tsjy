import { onMounted, ref } from 'vue'
import { getRoleApi } from '../apis/role'

export const useGetRole = () => {
  const loading = ref(false)

  const data = ref({
    total: 0,
    items: [],
  })

  const pageIndex = ref(1)
  const pageSize = ref(20)

  const getRole = async () => {
    if (loading.value) {
      return
    }

    loading.value = true

    try {
      const res = await getRoleApi(pageIndex.value, pageSize.value)
      data.value = res.data
    } catch (e) {
      console.error(e)
    } finally {
      loading.value = false
    }
  }

  onMounted(async () => {
    await getRole()
  })

  const handlePageChange = async () => {
    await getRole()
  }

  const handlePageSizeChange = async (size) => {
    pageSize.value = size
    await getRole()
  }

  return {
    loading,
    data,
    pageIndex,
    pageSize,
    getRole,
    handlePageChange,
    handlePageSizeChange,
  }
}
