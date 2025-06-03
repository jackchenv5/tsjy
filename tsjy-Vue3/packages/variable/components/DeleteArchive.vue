<script setup>
import { useDrawer } from '#/common/composables/drawer'
import { ElMessage, useLocale } from 'element-plus'
import { ref, watch } from 'vue'
import { deleteArchivedVariableApi } from '../apis/variableArchive'

const model = defineModel()
const props = defineProps(['archivedVariable'])
const emits = defineEmits(['finish'])

const { showDrawer, drawerTitle, drawerSize, openDrawer, closeDrawer } =
  useDrawer()
const { t } = useLocale()
watch(
  () => model.value,
  (val) => {
    if (val) {
      drawerTitle.value = t('el.variable.variableArchive.deleteArchiveVariable')
      openDrawer('300')
    }
  },
)

const onDrawerClosed = () => {
  model.value = false
}

const formData = ref({
  connectorInstance: '',
  connectionName: '',
  dataPoint: '',
  name: '',
})
watch(
  () => props.archivedVariable,
  (val) => {
    if (val) {
      formData.value.connectorInstance = val.connectorInstance
      formData.value.connectionName = val.connectionName
      formData.value.dataPoint = val.dataPoint
      formData.value.name = val.name
    }
  },
)
const loading = ref(false)

const deleteArchiveVariable = async () => {
  loading.value = true

  try {
    await deleteArchivedVariableApi(props.archivedVariable.id)
    formData.value = {
      connectorInstance: '',
      connectionName: '',
      dataPoint: '',
      name: '',
    }
    ElMessage({
      type: 'success',
      message: t(
        'el.variable.variableArchive.archivedVariableDeleteSuccessful',
      ),
    })
    model.value = false
    closeDrawer()
    emits('finish')
  } catch (error) {
    ElMessage({
      type: 'error',
      message: t('el.variable.variableArchive.archivedVariableDeleteFailed'),
    })
  } finally {
    loading.value = false
  }
}

const onDelete = async () => {
  await deleteArchiveVariable()
}
</script>

<template>
  <ElDrawer
    :key="new Date().getTime()"
    v-model="showDrawer"
    :title="drawerTitle"
    :size="drawerSize"
    @closed="onDrawerClosed"
  >
    <ElForm v-model="formData" label-position="top" v-loading="loading">
      <ElFormItem :label="t('el.variable.variableArchive.connectorInstance')">
        <ElInput v-model="formData.connectorInstance" disabled></ElInput>
      </ElFormItem>
      <ElFormItem :label="t('el.variable.variableArchive.connectionName')">
        <ElInput v-model="formData.connectionName" disabled></ElInput>
      </ElFormItem>
      <ElFormItem :label="t('el.variable.variableArchive.dataPointName')">
        <ElInput v-model="formData.dataPoint" disabled></ElInput>
      </ElFormItem>
      <ElFormItem :label="t('el.variable.variableArchive.name')">
        <ElInput v-model="formData.name" disabled></ElInput>
      </ElFormItem>
      <ElFormItem>
        <template #label>
          <p class="confirm-text">
            {{ t('el.variable.variableArchive.deleteArchiveConfirm') }}
          </p>
        </template>
        <ElButton type="danger" :loading="loading" @click="onDelete">
          {{ t('el.variable.variableArchive.delete') }}
        </ElButton>
      </ElFormItem>
    </ElForm>
  </ElDrawer>
</template>

<style scoped lang="scss">
.confirm-text {
  color: var(--el-color-danger);
}
</style>
