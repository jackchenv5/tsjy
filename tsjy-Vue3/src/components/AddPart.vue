<script setup>
import { computed, ref, watch } from 'vue'
import { useDrawer } from '#/common/composables/drawer.js'
import { ElMessage } from 'element-plus'
import SelectVariable from '#/variable/components/SelectVariable.vue'
import { addPartApi } from '@/apis/sawingPart.js'

// region 组件
const model = defineModel()
const emits = defineEmits(['finished'])
const props = defineProps(['facilityId'])

// endregion

// region 抽屉组件
const { showDrawer, drawerTitle, drawerSize, openDrawer, closeDrawer } =
  useDrawer()
watch(model, (val) => {
  if (val) {
    partFormData.value.facilityId = props.facilityId
    drawerTitle.value = '添加零件'
    openDrawer()
  }
})
watch(showDrawer, (val) => {
  model.value = val
})
// endregion

// region 零件数据
const partFormData = ref({
  partName: '',
  facilityId: null,
  remark: '',
  connectorInstance: '',
  connectionName: '',
  dataPoint: '',
  variableName: '',
})
watch(
  () => props.facilityId,
  (val) => {
    partFormData.value.facilityId = val
  },
)
// endregion

// region 添加零件
const formRef = ref()
const formRules = ref({
  partName: [{ trigger: 'blur', required: true, message: '请输入零件名称' }],
})
const loadingAddPart = ref(false)
const onAddClick = async () => {
  await formRef.value.validate(async (valid) => {
    if (valid) {
      await addPart()
    }
  })
}
const addPart = async () => {
  if (loadingAddPart.value) {
    return
  }
  loadingAddPart.value = true
  try {
    await addPartApi([partFormData.value])
    ElMessage({
      type: 'success',
      message: '添加零件成功',
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
      message: '添加零件失败',
    })
    console.error(e)
  } finally {
    loadingAddPart.value = false
  }
}
// endregion

// region 选择变量
const showSelectVariable = ref(false)
const onSelectVariableClick = () => {
  showSelectVariable.value = true
}
const onVariableSelect = (variable) => {
  partFormData.value.connectorInstance = variable.connectorInstance
  partFormData.value.connectionName = variable.connectionName
  partFormData.value.dataPoint = variable.dataPointName
  partFormData.value.variableName = variable.name
  showSelectVariable.value = false
}
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
    <ElForm
      :model="partFormData"
      label-position="top"
      ref="formRef"
      :rules="formRules"
      @submit.prevent
    >
      <ElFormItem prop="partName" required label="零件名称">
        <ElInput v-model="partFormData.partName"></ElInput>
      </ElFormItem>
      <ElFormItem prop="remark" label="备注">
        <ElInput v-model="partFormData.remark"></ElInput>
      </ElFormItem>
      <ElFormItem label="变量" prop="variableDisplay">
        <ElInput readonly v-model="variableDisplay" type="textarea" :rows="4">
        </ElInput>
        <ElButton
          style="margin-top: 1rem"
          type="primary"
          plain
          @click="onSelectVariableClick"
        >
          选择变量
        </ElButton>
      </ElFormItem>
      <ElFormItem>
        <ElButton type="primary" @click="onAddClick">添加</ElButton>
      </ElFormItem>
    </ElForm>
  </ElDrawer>

  <SelectVariable v-model="showSelectVariable" @select="onVariableSelect">
  </SelectVariable>
</template>

<style scoped lang="scss"></style>
