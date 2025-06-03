<script setup>
import { useRouter } from 'vue-router'
import { computed } from 'vue'

const props = defineProps(['modelValue'])
const emits = defineEmits(['update:modelValue'])

const router = useRouter()

const paths = computed(() => {
  const routes = router.getRoutes()
  let all = []
  for (let i = 0; i < routes.length; i++) {
    const path = routes[i].path
    if (!all.includes(path)) {
      if (path === '/404') {
        continue
      }
      if (path === '/login') {
        continue
      }
      all.push(path)
    }
  }
  return all.sort()
})

const value = computed({
  get: () => {
    return props.modelValue
  },
  set: (val) => {
    emits('update:modelValue', val)
  },
})
</script>

<template>
  <ElSelect v-model="value" filterable>
    <ElOption
      v-for="(path, index) of paths"
      :key="index"
      :value="path"
    ></ElOption>
  </ElSelect>
</template>

<style scoped lang="scss"></style>
