<script setup>
import { useDrawer } from '#/common/composables/drawer.js'
import { computed, ref, watch } from 'vue'
import { useFacilities } from '#/facility/composables/facilities.js'
import SelectVariable from '#/variable/components/SelectVariable.vue'
import { addAlarmDefinitionApi } from '@/apis/sawingMachineAlarm.js'
import { ElMessage } from 'element-plus'

// region 组件
const model = defineModel()
const emits = defineEmits(['finished'])
// endregion

// region 抽屉组件
const { showDrawer, drawerTitle, drawerSize, openDrawer, closeDrawer } =
  useDrawer()
watch(model, (val) => {
  if (val) {
    drawerTitle.value = '添加报警定义'
    openDrawer()
  }
})
watch(showDrawer, (val) => {
  model.value = val
})
// endregion

// region 处理表单
const formData = ref({
  facilityId: null,
  connectorInstance: '',
  connectionName: '',
  dataPointName: '',
  name: '',
  dataType: 'int',
  triggerType: 2,
  triggerValue: 0,
  message: '',
})
const variableDisplay = computed(() => {
  if (formData.value.name) {
    return `${formData.value.connectorInstance} -> ${formData.value.connectionName} -> ${formData.value.dataPointName} -> ${formData.value.name}`
  } else {
    return ''
  }
})
const formRef = ref()
const formRules = ref({
  facilityId: [
    {
      trigger: 'blur',
      required: true,
      message: '请选择设备',
    },
  ],
  variableDisplay: [
    {
      validator: (rule, value, callback) => {
        if (variableDisplay.value) {
          callback()
        } else {
          callback(new Error('请选择变量'))
        }
      },
      trigger: 'change',
      required: true,
    },
  ],
  message: [
    {
      trigger: 'blur',
      required: true,
      message: '请输入报警消息',
    },
  ],
})

// endregion

// region 数据类型变化
const triggerValuePrecision = ref(0)
const handleDataTypeChange = () => {
  console.log(formData.value['dataType'])
  if (formData.value['dataType'] === 'int') {
    triggerValuePrecision.value = 0
  } else {
    triggerValuePrecision.value = 6
  }
}
// endregion

// region 设备列表
const { enabledFacilities } = useFacilities()
// endregion

// region 选择变量
const showSelectVariable = ref(false)
const handleShowSelectVariable = () => {
  showSelectVariable.value = true
}
const handleVariableSelect = (variable) => {
  formData.value.connectorInstance = variable.connectorInstance
  formData.value.connectionName = variable.connectionName
  formData.value.dataPointName = variable.dataPointName
  formData.value.name = variable.name
  showSelectVariable.value = false
}
// endregion

// region 添加
const handleAdd = async () => {
  await formRef.value.validate(async (valid) => {
    if (valid) {
      await addAlarmDefinition()
    }
  })
}
const loadingAddAlarmDefinition = ref(false)
const addAlarmDefinition = async () => {
  if (loadingAddAlarmDefinition.value) {
    return
  }
  loadingAddAlarmDefinition.value = true
  try {
    await addAlarmDefinitionApi({
      connectorInstance: formData.value['connectorInstance'],
      connectionName: formData.value['connectionName'],
      dataPoint: formData.value['dataPointName'],
      name: formData.value['name'],
      facilityId: formData.value['facilityId'],
      message: formData.value['message'],
      triggerType: formData.value['triggerType'],
      dataType: formData.value['dataType'],
      triggerValue: formData.value['triggerValue'].toString(),
    })
    ElMessage({
      message: '添加报警定义成功',
      type: 'success',
    })
    closeDrawer()
    emits('finished')
  } catch (e) {
    ElMessage({
      message: '添加报警定义失败',
      type: 'error',
    })
    console.error(e)
  } finally {
    loadingAddAlarmDefinition.value = false
  }
}
// endregion
</script>

<template>
  <ElDrawer v-model="showDrawer" :title="drawerTitle" :size="drawerSize">
    <ElForm
      :model="formData"
      label-position="top"
      ref="formRef"
      :rules="formRules"
    >
      <ElFormItem label="选择设备" prop="facilityId">
        <ElSelect v-model="formData['facilityId']">
          <ElOption
            v-for="facility of enabledFacilities"
            :label="facility['name']"
            :value="facility['id']"
            :key="facility['id']"
          >
          </ElOption>
        </ElSelect>
      </ElFormItem>
      <ElFormItem label="变量" prop="variableDisplay">
        <ElInput readonly v-model="variableDisplay" type="textarea" :rows="4">
        </ElInput>
        <ElButton
          style="margin-top: 1rem"
          type="primary"
          plain
          @click="handleShowSelectVariable"
        >
          选择变量
        </ElButton>
      </ElFormItem>
      <ElFormItem label="数据类型">
        <ElSelect v-model="formData['dataType']" @change="handleDataTypeChange">
          <ElOption value="int" label="整数"></ElOption>
          <ElOption value="float" label="浮点数"></ElOption>
        </ElSelect>
      </ElFormItem>
      <ElFormItem label="触发条件">
        <div style="display: flex; width: 100%; grid-gap: 1rem">
          <ElSelect style="flex: 1" v-model="formData['triggerType']">
            <ElOption :value="0" label=">"></ElOption>
            <ElOption :value="1" label="<"></ElOption>
            <ElOption :value="2" label="="></ElOption>
            <ElOption :value="3" label="≠"></ElOption>
            <ElOption :value="4" label="≥"></ElOption>
            <ElOption :value="5" label="≤"></ElOption>
          </ElSelect>
          <ElInputNumber
            style="flex: 2"
            v-model="formData['triggerValue']"
            :precision="triggerValuePrecision"
          >
          </ElInputNumber>
        </div>
      </ElFormItem>
      <ElFormItem label="报警消息" prop="message">
        <ElInput type="textarea" :rows="5" v-model="formData['message']">
        </ElInput>
      </ElFormItem>
      <ElFormItem>
        <ElButton
          type="primary"
          @click="handleAdd"
          :loading="loadingAddAlarmDefinition"
        >
          添加
        </ElButton>
      </ElFormItem>
    </ElForm>
  </ElDrawer>

  <SelectVariable v-model="showSelectVariable" @select="handleVariableSelect">
  </SelectVariable>
</template>

<style scoped lang="scss"></style>
