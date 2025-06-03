import { defineStore } from 'pinia'
import { onMounted, ref } from 'vue'
import { getCurrentShiftApi } from '../apis/shift'

export const useShiftStore = defineStore('shift', () => {
  const currentShift = ref()
  const loadingCurrentShift = ref(false)
  const getCurrentShift = async () => {
    loadingCurrentShift.value = true
    try {
      const { data } = await getCurrentShiftApi()
      currentShift.value = data
    } catch (e) {
      console.error(e)
    } finally {
      loadingCurrentShift.value = false
    }
  }
  let interval
  let lastMinute = 0
  const minuteChange = () => {
    const now = new Date().getMinutes()
    if (now !== lastMinute) {
      lastMinute = now
      return true
    }
    return false
  }
  const startInterval = () => {
    stopInterval()
    interval = setInterval(async () => {
      if (minuteChange()) {
        await getCurrentShift()
      }
    }, 1000)
  }
  const stopInterval = () => {
    if (interval) {
      clearInterval(interval)
    }
  }
  onMounted(async () => {
    await getCurrentShift()
  })

  return { currentShift, getCurrentShift, startInterval, stopInterval }
})
