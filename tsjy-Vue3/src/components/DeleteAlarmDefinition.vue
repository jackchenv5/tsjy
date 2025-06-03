<script setup>
import { useDrawer } from '#/common/composables/drawer.js'
import { computed, ref, watch } from 'vue'
import { useFacilities } from '#/facility/composables/facilities.js'
import SelectVariable from '#/variable/components/SelectVariable.vue'
import { deleteAlarmDefinitionApi } from '@/apis/sawingMachineAlarm.js'
import { ElMessage } from 'element-plus'

// region 组件
const model = defineModel()
const emits = defineEmits(['finished'])
const props = defineProps(['alarmDefinition'])
// endregion

// region 抽屉组件
const { showDrawer, drawerTitle, drawerSize, openDrawer, closeDrawer } =
  useDrawer()
watch(model, (val) => {
  if (val) {
    if (props.alarmDefinition) {
      const definition = props.alarmDefinition
      formData.value.id = definition.id
      formData.value.facilityId = definition.facilityId
      formData.value.connectorInstance = definition.connectorInstance
      formData.value.connectionName = definition.connectionName
      formData.value.dataPoint = definition.dataPoint
      formData.value.name = definition.name
      formData.value.dataType = definition.dataType
      handleDataTypeChange()
      formData.value.triggerType = definition.triggerType
      formData.value.triggerValue = definition.triggerValue
      formData.value.message = definition.message
    }

    drawerTitle.value = '删除报警定义'
    openDrawer()
  }
})
watch(showDrawer, (val) => {
  model.value = val
})
// endregion

// region 处理表单
const formData = ref({
  id: 0,
  facilityId: null,
  connectorInstance: '',
  connectionName: '',
  dataPoint: '',
  name: '',
  dataType: 'int',
  triggerType: 2,
  triggerValue: 0,
  message: '',
})

const variableDisplay = computed(() => {
  if (formData.value.name) {
    return `${formData.value.connectorInstance} -> ${formData.value.connectionName} -> ${formData.value.dataPoint} -> ${formData.value.name}`
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
  if (formData.value['dataType'] === 'int') {
    triggerValuePrecision.value = 0
  } else {
    triggerValuePrecision.value = undefined
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
  formData.value.dataPoint = variable.dataPointName
  formData.value.name = variable.name
  showSelectVariable.value = false
}
// endregion

// region 添加
const handleDelete = async () => {
  await formRef.value.validate(async (valid) => {
    if (valid) {
      await deleteAlarmDefinition()
    }
  })
}
const loadingDeleteAlarmDefinition = ref(false)
const deleteAlarmDefinition = async () => {
  if (loadingDeleteAlarmDefinition.value) {
    return
  }
  loadingDeleteAlarmDefinition.value = true
  try {
    await deleteAlarmDefinitionApi(formData.value['id'])
    ElMessage({
      message: '删除报警定义成功',
      type: 'success',
    })
    closeDrawer()
    emits('finished')
  } catch (e) {
    ElMessage({
      message: '删除报警定义失败',
      type: 'error',
    })
    console.error(e)
  } finally {
    loadingDeleteAlarmDefinition.value = false
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
        <ElSelect v-model="formData['facilityId']" disabled>
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
        <ElInput
          readonly
          v-model="variableDisplay"
          type="textarea"
          :rows="4"
          disabled
        >
        </ElInput>
        <ElButton
          style="margin-top: 1rem"
          type="primary"
          plain
          @click="handleShowSelectVariable"
          disabled
        >
          选择变量
        </ElButton>
      </ElFormItem>
      <ElFormItem label="数据类型">
        <ElSelect
          v-model="formData['dataType']"
          @change="handleDataTypeChange"
          disabled
        >
          <ElOption value="int" label="整数"></ElOption>
          <ElOption value="float" label="浮点数"></ElOption>
        </ElSelect>
      </ElFormItem>
      <ElFormItem label="触发条件">
        <div style="display: flex; width: 100%; grid-gap: 1rem">
          <ElSelect style="flex: 1" v-model="formData['triggerType']" disabled>
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
            disabled
          >
          </ElInputNumber>
        </div>
      </ElFormItem>
      <ElFormItem label="报警消息" prop="message">
        <ElInput
          type="textarea"
          :rows="5"
          v-model="formData['message']"
          disabled
        >
        </ElInput>
      </ElFormItem>
      <ElFormItem>
        <template #label>
          <p class="confirm-text">删除后无法恢复，确定要删除设备吗？</p>
        </template>
        <ElButton
          type="danger"
          @click="handleDelete"
          :loading="loadingDeleteAlarmDefinition"
        >
          删除
        </ElButton>
      </ElFormItem>
    </ElForm>
  </ElDrawer>

  <SelectVariable v-model="showSelectVariable" @select="handleVariableSelect">
  </SelectVariable>
</template>

<style scoped lang="scss">
.confirm-text {
  color: var(--el-color-danger);
}
</style>
