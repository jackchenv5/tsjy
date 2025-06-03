<script setup>
import ContentLayout from '#/common/layouts/ContentLayout.vue'
import FacilityList from '#/facility/components/FacilityList.vue'
import { ref } from 'vue'
import {
  addCraftGroupApi,
  deleteCraftGroupApi,
  getCraftBindingsApi,
  updateCraftBindingApi,
} from '@/apis/sawingCraft.js'
import UnbindingVariable from '#/variable/components/UnbindingVariable.vue'
import SelectVariable from '#/variable/components/SelectVariable.vue'
import { ElMessage } from 'element-plus'

// region 当前设备
const currentFacility = ref()
const onFacilityChange = async (facility) => {
  currentFacility.value = facility
  await getBindings()
}
// endregion

// region 获取绑定数据
const loadingGetBindings = ref(false)
const bindings = ref()
const getBindings = async () => {
  loadingGetBindings.value = true
  try {
    const { data } = await getCraftBindingsApi(currentFacility.value['id'])
    bindings.value = data
  } catch (e) {
    console.error(e)
  } finally {
    loadingGetBindings.value = false
  }
}
const getBindingTypeString = (bindingType) => {
  switch (bindingType) {   
    case 1:
      return '段高度'
    case 2:
      return '段进给速度'
    case 3:
      return '新线进给量'
    case 4:
      return '段线速度'
  }
}
// endregion

// region 更新绑定
const loadingUpdateBinding = ref(false)
const updateBinding = async (binding) => {
  loadingUpdateBinding.value = true
  try {
    await updateCraftBindingApi(binding)
    ElMessage({
      type: 'success',
      message: '绑定更新成功',
    })
    await getBindings()
  } catch (e) {
    ElMessage({
      type: 'error',
      message: '绑定更新失败',
    })
    console.error(e)
  } finally {
    loadingUpdateBinding.value = false
  }
}
// endregion

// region 绑定
const showSelectVariable = ref(false)
const currentBinding = ref()
const onBinding = (binding) => {
  currentBinding.value = binding
  showSelectVariable.value = true
}
const onVariableSelect = async (value) => {
  const binding = {
    id: currentBinding.value['id'],
    facilityId: currentBinding.value['facilityId'],
    index: currentBinding.value['index'],
    bindingType: currentBinding.value['bindingType'],
    connectorInstance: value['connectorInstance'],
    connectionName: value['connectionName'],
    dataPoint: value['dataPointName'],
    name: value['name'],
  }
  await updateBinding(binding)
  currentBinding.value = null
}
// endregion

// region 解绑
const showUnbindingConfirm = ref(false)
const onConfirmUnbinding = async () => {
  const binding = {
    id: currentBinding.value['id'],
    facilityId: currentBinding.value['facilityId'],
    index: currentBinding.value['index'],
    bindingType: currentBinding.value['bindingType'],
    connectorInstance: '',
    connectionName: '',
    dataPoint: '',
    name: '',
  }
  await updateBinding(binding)
}
const onUnbinding = async (binding) => {
  currentBinding.value = binding
  showUnbindingConfirm.value = true
}
// endregion

// region 添加一组
const loadingAddGroup = ref(false)
const addGroup = async () => {
  if (loadingAddGroup.value) {
    return
  }
  loadingAddGroup.value = true
  try {
    await addCraftGroupApi(currentFacility.value['id'])
    ElMessage({
      type: 'success',
      message: '添加工艺变量成功',
    })
  } catch (e) {
    console.error(e)
    ElMessage({
      type: 'error',
      message: '添加工艺变量失败',
    })
  } finally {
    loadingAddGroup.value = false
  }
}
const onAddGroupClick = async () => {
  await addGroup()
  await getBindings()
}
// endregion

// region 删除一组
const loadingDeleteGroup = ref(false)
const deleteGroup = async () => {
  if (loadingDeleteGroup.value) {
    return
  }
  loadingDeleteGroup.value = true
  try {
    await deleteCraftGroupApi(currentFacility.value['id'])
    ElMessage({
      type: 'success',
      message: '删除工艺变量成功',
    })
  } catch (e) {
    console.error(e)
    ElMessage({
      type: 'error',
      message: '删除工艺变量失败',
    })
  } finally {
    loadingDeleteGroup.value = false
  }
}
const onDeleteGroupClick = async () => {
  await deleteGroup()
  await getBindings()
}
// endregion
</script>

<template>
  <ContentLayout>
    <template #header>工艺配置</template>
    <template #default>
      <FacilityList @change="onFacilityChange"></FacilityList>
      <div>
        <ElButton type="primary" plain @click="onAddGroupClick">
          添加一组
        </ElButton>
        <ElButton type="danger" plain @click="onDeleteGroupClick">
          删除一组
        </ElButton>
      </div>
      <ElTable
        stripe
        show-overflow-tooltip
        :data="bindings"
        row-key="id"
        default-expand-all
      >
        <ElTableColumn prop="bindingType" label="序号" min-width="160">
          <template #default="{ row }">
            <template v-if="row['id'] < 0">
              {{ row['index'] }}
            </template>
            <template v-else>
              {{ getBindingTypeString(row['bindingType']) }}
            </template>
          </template>
        </ElTableColumn>
        <ElTableColumn label="连接器实例" prop="connectorInstance" width="170">
        </ElTableColumn>
        <ElTableColumn label="连接名称" prop="connectionName" min-width="160">
        </ElTableColumn>
        <ElTableColumn label="数据点名称" prop="dataPoint" width="150">
        </ElTableColumn>
        <ElTableColumn label="变量名称" prop="name" min-width="240">
        </ElTableColumn>
        <ElTableColumn width="170">
          <template #default="{ row }">
            <ElButtonGroup size="small" v-if="row['id'] > 0">
              <ElButton type="primary" plain @click="onBinding(row)">
                绑定
              </ElButton>
              <ElButton type="warning" plain @click="onUnbinding(row)">
                解绑
              </ElButton>
            </ElButtonGroup>
          </template>
        </ElTableColumn>
      </ElTable>
    </template>
  </ContentLayout>
  <SelectVariable v-model="showSelectVariable" @select="onVariableSelect">
  </SelectVariable>
  <UnbindingVariable
    v-model="showUnbindingConfirm"
    @finished="onConfirmUnbinding"
  >
  </UnbindingVariable>
</template>

<style lang="scss">
.type-row {
  --el-table-tr-bg-color: #ffbb96;
}
</style>
