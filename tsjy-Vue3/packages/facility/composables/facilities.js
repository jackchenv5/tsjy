import { onMounted, ref } from 'vue'
import { getFacilitiesApi } from '../apis/facility'

export const useFacilities = () => {
  const facilities = ref([])
  const loadingFacilities = ref(false)

  const getFacilities = async () => {
    if (loadingFacilities.value) {
      return
    }

    loadingFacilities.value = true
    try {
      const { data } = await getFacilitiesApi()
      facilities.value = data
      getEnabledFacilities()
    } catch (e) {
      console.error(e)
    } finally {
      loadingFacilities.value = false
    }
  }

  onMounted(async () => {
    await getFacilities()
  })

  const enabledFacilities = ref([])
  const getEnabledFacilities = () => {
    if (facilities.value.length === 0) {
      enabledFacilities.value = []
      return
    }
    enabledFacilities.value = facilities.value.filter(
      (item) => item['isEnabled'],
    )
  }

  return { facilities, loadingFacilities, getFacilities, enabledFacilities }
}
