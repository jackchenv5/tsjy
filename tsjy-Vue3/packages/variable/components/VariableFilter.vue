<script setup>
import { useLocale } from 'element-plus'
import { ref } from 'vue'
import { Filter } from '@element-plus/icons-vue'

defineProps(['loading'])
const emits = defineEmits(['filter'])
const { t } = useLocale()

const filters = ref({
  name: '',
})
const handleFilter = () => {
  emits('filter', filters.value)
}
</script>

<template>
  <div class="variable-filter-container">
    <ElInput
      class="filter-item"
      v-model="filters.name"
      :placeholder="t('el.variable.variableArchive.nameFilterPlaceholder')"
      clearable
    >
    </ElInput>
    <ElButton
      type="primary"
      plain
      :icon="Filter"
      @click="handleFilter"
      :loading="loading"
    >
      {{ t('el.variable.variableArchive.filter') }}
    </ElButton>
  </div>
</template>

<style scoped lang="scss">
.variable-filter-container {
  display: flex;
  flex-flow: row wrap;
  grid-gap: 1rem;
  align-items: center;

  > .filter-item {
    width: 20rem;
  }
}
</style>
