import { onBeforeMount, onBeforeUnmount, ref } from 'vue'

const sm = 576
const md = 768
const lg = 992
const xl = 1200
const xxl = 1400

export { sm, md, lg, xl, xxl }

export const useResize = (options) => {
  const width = ref(0)
  const breakPoint = ref('')

  const buildBreakPoint = () => {
    if (width.value >= xxl) {
      breakPoint.value = 'xxl'
      return
    }
    if (width.value >= xl) {
      breakPoint.value = 'xl'
      return
    }
    if (width.value >= lg) {
      breakPoint.value = 'lg'
      return
    }
    if (width.value >= md) {
      breakPoint.value = 'md'
      return
    }
    if (width.value >= sm) {
      breakPoint.value = 'sm'
      return
    }
    breakPoint.value = 'xs'
  }

  const resizeHandler = () => {
    const innerWidth = window.innerWidth
    width.value = innerWidth
    buildBreakPoint()

    const onResize = options?.onResize
    if (onResize) {
      onResize(innerWidth)
    }
  }

  onBeforeMount(() => {
    // 页面挂载前先执行一次以设定初始状态
    resizeHandler()
    window.addEventListener('resize', resizeHandler)
  })

  onBeforeUnmount(() => {
    window.removeEventListener('resize', resizeHandler)
  })

  return { width, breakPoint }
}
