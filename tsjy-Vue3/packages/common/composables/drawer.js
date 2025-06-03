import { ref } from 'vue'
import { md, useResize } from './resize'

export const useDrawer = () => {
  const showDrawer = ref(false)
  const drawerTitle = ref('')
  const drawerSize = ref('30%')
  const customSize = ref('30%')

  const openDrawer = (size = '30%') => {
    drawerSize.value = size
    customSize.value = size

    resize()

    showDrawer.value = true
  }

  const closeDrawer = () => {
    showDrawer.value = false
  }

  const resize = () => {
    if (width.value < md) {
      drawerSize.value = '100%'
    } else {
      drawerSize.value = customSize.value
    }
  }

  const { width } = useResize({
    onResize: () => {
      resize()
    },
  })

  return { showDrawer, drawerTitle, drawerSize, openDrawer, closeDrawer }
}
