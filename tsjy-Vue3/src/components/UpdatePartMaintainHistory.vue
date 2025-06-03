<script setup>
import { computed, ref, watch } from 'vue'
import { useDrawer } from '#/common/composables/drawer.js'
import { ElMessage,dayjs } from 'element-plus'
import advancedFormat from 'dayjs/plugin/advancedFormat'
import SelectVariable from '#/variable/components/SelectVariable.vue'
import { updatePartHistoriesApi } from '@/apis/sawingPart.js'

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
    drawerTitle.value = '更新零件'
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

// region 更新零件
const formRef = ref()
const formRules = ref({
  partName: [{ trigger: 'blur', required: true, message: '请输入零件名称' }],
})
const loadingUpdatePart = ref(false)
const onUpdateClick = async () => {
  await formRef.value.validate(async (valid) => {
    if (valid) {
      await updatePart()
    }
  })
}
const updatePart = async () => {
  if (loadingUpdatePart.value) {
    return
  }
  loadingUpdatePart.value = true
  try {
    await updatePartHistoriesApi(partFormData.value)
    ElMessage({
      type: 'success',
      message: '更新零件成功',
    })
    partFormData.value = {
      ProcessTime: '',
      Time: '',
      facilityId: null,
      remark: '',
      PartName: '',
      Reason: '',
      ProcessMethod: '',
      id: 0,

    }
    closeDrawer()
    emits('finished')
  } catch (e) {
    ElMessage({
      type: 'error',
      message: '更新零件失败',
    })
    console.error(e)
  } finally {
    loadingUpdatePart.value = false
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
    <ElForm :model="partFormData" label-position="top" ref="formRef" :rules="formRules" @submit.prevent>
      <ElFormItem prop="partName" label="零件名称">
        <ElInput v-model="partFormData.partName" readonly></ElInput>
      </ElFormItem>
      <ElFormItem label="零件备注" prop="remark">
        <ElInput v-model="partFormData.remark"></ElInput>
      </ElFormItem>
      <ElFormItem label="维护原因" prop="reason">
        <ElInput v-model="partFormData.reason" type="textarea" :rows="2"></ElInput>
      </ElFormItem>
      <ElFormItem label="处理方法" prop="processMethod">
        <ElInput v-model="partFormData.processMethod" type="textarea" :rows="2">
        </ElInput>
      </ElFormItem>
      <ElFormItem label="处理时间" prop="processTime">
        <ElDatePicker type="datetime" value-format="X" v-model="partFormData.processTime" >
        </ElDatePicker>
      </ElFormItem>

      <ElFormItem>
        <ElButton type="primary" @click="onUpdateClick">更新</ElButton>
      </ElFormItem>
    </ElForm>
  </ElDrawer>

  <SelectVariable v-model="showSelectVariable" @select="onVariableSelect">
  </SelectVariable>
</template>

<style scoped lang="scss"></style>
