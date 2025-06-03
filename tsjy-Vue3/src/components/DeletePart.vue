<script setup>
import { computed, ref, watch } from 'vue'
import { useDrawer } from '#/common/composables/drawer.js'
import { ElMessage } from 'element-plus'
import { deletePartApi } from '@/apis/sawingPart.js'

// region 组件
const model = defineModel()
const emits = defineEmits(['finished'])
const props = defineProps(['part'])

// endregion

// region 抽屉组件
const { showDrawer, drawerTitle, drawerSize, openDrawer, closeDrawer } =
  useDrawer()
watch(model, (val) => {
  if (val) {
    partFormData.value = props.part
    drawerTitle.value = '删除零件'
    openDrawer()
  }
})
watch(showDrawer, (val) => {
  model.value = val
})
// endregion

// region 零件数据
const partFormData = ref()
// endregion

// region 删除零件
const loadingDeletePart = ref(false)
const onDeleteClick = async () => {
  await deletePart()
}
const deletePart = async () => {
  if (loadingDeletePart.value) {
    return
  }
  loadingDeletePart.value = true
  try {
    await deletePartApi(partFormData.value.id)
    ElMessage({
      type: 'success',
      message: '删除零件成功',
    })
    partFormData.value = {
      partName: '',
      facilityId: null,
      remark: '',
      connectorInstance: '',
      connectionName: '',
      dataPoint: '',
      variableName: '',
    }
    closeDrawer()
    emits('finished')
  } catch (e) {
    ElMessage({
      type: 'error',
      message: '删除零件失败',
    })
    console.error(e)
  } finally {
    loadingDeletePart.value = false
  }
}
// endregion

// region 选择变量
const variableDisplay = computed(() => {
  if (partFormData.value.variableName) {
    return (
      `${partFormData.value.connectorInstance} -> ` +
      `${partFormData.value.connectionName} -> ` +
      `${partFormData.value.dataPoint} -> ` +
      `${partFormData.value.variableName}`
    )
  } else {
    return ''
  }
})
// endregion
</script>

<template>
  <ElDrawer v-model="showDrawer" :size="drawerSize" :title="drawerTitle">
    <ElForm :model="partFormData" label-position="top" @submit.prevent>
      <ElFormItem prop="partName" required label="零件名称">
        <ElInput v-model="partFormData.partName" readonly></ElInput>
      </ElFormItem>
      <ElFormItem prop="remark" label="备注">
        <ElInput v-model="partFormData.remark" readonly></ElInput>
      </ElFormItem>
      <ElFormItem label="变量" prop="variableDisplay">
        <ElInput readonly v-model="variableDisplay" type="textarea" :rows="4">
        </ElInput>
      </ElFormItem>
      <ElFormItem>
        <template #label>
          <p class="confirm-text">删除后无法恢复，确定要删除吗？</p>
        </template>
        <ElButton type="danger" @click="onDeleteClick">删除</ElButton>
      </ElFormItem>
    </ElForm>
  </ElDrawer>
</template>

<style scoped lang="scss">
.confirm-text {
  color: var(--el-color-danger);
}
</style>
