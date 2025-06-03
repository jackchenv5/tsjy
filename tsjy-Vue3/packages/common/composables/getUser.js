import { onMounted, ref } from 'vue'
import { getUserApi } from '../apis/user'

export const useGetUser = () => {
  const loading = ref(false)

  const data = ref({
    total: 0,
    items: [],
  })

  const pageIndex = ref(1)
  const pageSize = ref(20)

  const getUser = async () => {
    if (loading.value) {
      return
    }

    loading.value = true

    try {
      const res = await getUserApi(pageIndex.value, pageSize.value)
      data.value = res.data
    } catch (e) {
      console.error(e)
    } finally {
      loading.value = false
    }
  }

  onMounted(async () => {
    await getUser()
  })

  const handlePageChange = async () => {
    await getUser()
  }

  const handlePageSizeChange = async (size) => {
    pageSize.value = size
    await getUser()
  }

  return {
    loading,
    data,
    pageIndex,
    pageSize,
    getUser,
    handlePageChange,
    handlePageSizeChange,
  }
}
