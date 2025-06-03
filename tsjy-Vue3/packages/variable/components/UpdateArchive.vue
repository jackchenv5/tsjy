<script setup>
import { useDrawer } from '#/common/composables/drawer'
import { ElMessage, useLocale } from 'element-plus'
import { ref, watch } from 'vue'
import { updateArchivedVariableApi } from '../apis/variableArchive'

const props = defineProps(['modelValue', 'archivedVariable'])
const emits = defineEmits(['update:modelValue', 'finish'])
const key = ref(0)

const { showDrawer, drawerTitle, drawerSize, openDrawer, closeDrawer } =
  useDrawer()
const { t } = useLocale()
watch(
  () => props.modelValue,
  (val) => {
    if (val) {
      drawerTitle.value = t('el.variable.variableArchive.updateArchive')
      openDrawer('300')
    }
  },
)
watch(showDrawer, (val) => {
  emits('update:modelValue', val)
})

const formData = ref({
  id: 0,
  archiveMode: 0,
  archiveInterval: 60,
})
watch(
  () => props.archivedVariable,
  (val) => {
    if (val) {
      formData.value.id = props.archivedVariable.id
      formData.value.archiveMode = props.archivedVariable.archiveMode
      formData.value.archiveInterval = props.archivedVariable.archiveInterval
    }
  },
)
const handleDrawerClosed = () => {
  key.value++
}
const loading = ref(false)
const updateArchiveVariable = async () => {
  if (loading.value) {
    return
  }
  loading.value = true

  try {
    await updateArchivedVariableApi(
      formData.value.id,
      formData.value.archiveInterval,
    )
    ElMessage({
      message: t(
        'el.variable.variableArchive.archivedVariableUpdateSuccessful',
      ),
      type: 'success',
    })
    closeDrawer()
    emits('finish')
  } catch (e) {
    ElMessage({
      message: t('el.variable.variableArchive.archivedVariableUpdateFailed'),
      type: 'error',
    })
    console.error(e)
  } finally {
    loading.value = false
  }
}
const handleUpdate = async () => {
  await updateArchiveVariable()
}
</script>

<template>
  <ElDrawer
    v-model="showDrawer"
    :title="drawerTitle"
    :size="drawerSize"
    @closed="handleDrawerClosed"
  >
    <ElForm
      :key="key"
      v-model="formData"
      label-position="top"
      @keydown.enter.prevent="handleUpdate"
      v-loading="loading"
    >
      <ElFormItem
        :label="t('el.variable.variableArchive.archiveInterval')"
        prop="archiveInterval"
        v-if="formData.archiveMode === 1"
      >
        <div class="interval-container">
          <ElInputNumber v-model="formData.archiveInterval" :min="1">
          </ElInputNumber>
          <ElText>
            {{ t('el.variable.variableArchive.archiveIntervalUnit') }}
          </ElText>
        </div>
      </ElFormItem>
      <ElFormItem>
        <ElButton type="primary" @click="handleUpdate">
          {{ t('el.variable.variableArchive.update') }}
        </ElButton>
      </ElFormItem>
    </ElForm>
  </ElDrawer>
</template>

<style scoped lang="scss">
.interval-container {
  display: flex;
  flex-flow: row;
  grid-gap: 1rem;
}
</style>
