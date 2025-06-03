import { defineStore } from 'pinia'
import { ref } from 'vue'

export const useMenuStateStore = defineStore('menu', () => {
  // 是否用户主动关闭菜单
  const closedByUser = ref(false)
  // 是否通过尺寸关闭菜单
  const closedBySize = ref(false)
  // 菜单是否展开
  const isExpanded = ref(true)

  function setClosedByUser(value = false) {
    if (!closedBySize.value) {
      // 没有通过尺寸关闭菜单，处理用户主动关闭菜单状态
      closedByUser.value = value
      // 菜单的展开状态应与通过按钮关闭状态相反
      isExpanded.value = !value
    } else {
      // 通过尺寸关闭了菜单，仅切换展开状态，不处理用户主动关闭状态
      isExpanded.value = !isExpanded.value
    }
  }

  function setClosedBySize(value = false) {
    // 无论如何，都应处理是否通过尺寸关闭了菜单
    closedBySize.value = value
    if (closedBySize.value) {
      // 如果通过尺寸关闭菜单，无论是否用户主动关闭菜单，都应折叠菜单
      isExpanded.value = false
    } else {
      // 如果没有通过尺寸关闭菜单，需要根据是否用户主动关闭菜单，处理是否折叠菜单

      // 如果用户主动关闭菜单，应折叠菜单
      // 否则，菜单应展开
      isExpanded.value = !closedByUser.value
    }
  }

  return { isExpanded, closedBySize, setClosedByUser, setClosedBySize }
})
