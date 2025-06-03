<script setup>
import { useDrawer } from '../composables/drawer'
import { ElMessage, useLocale } from 'element-plus'
import { computed, ref, watch } from 'vue'
import {
  getRolePermissionApi,
  updateRolePermissionApi,
} from '../apis/rolePermission'

const props = defineProps(['modelValue', 'role'])
const emits = defineEmits(['update:modelValue'])

const { showDrawer, drawerTitle, drawerSize, openDrawer, closeDrawer } =
  useDrawer()
const { t } = useLocale()

watch(
  () => props.modelValue,
  async (val) => {
    if (val) {
      drawerTitle.value = t('el.common.role.rolePermission')
      openDrawer('100%')
      await getRolePermission()
    }
  },
)

watch(showDrawer, (val) => {
  emits('update:modelValue', val)
})

const loading = ref(false)
const tableData = ref()
const getRolePermission = async () => {
  if (loading.value) {
    return
  }

  loading.value = true

  try {
    const res = await getRolePermissionApi(props.role.id)
    tableData.value = res.data
  } catch (e) {
    console.error(e)
  } finally {
    loading.value = false
  }
}

const getMethodTagType = (httpMethod) => {
  switch (httpMethod) {
    case 'GET':
      return 'primary'
    case 'POST':
      return 'success'
    case 'PUT':
      return 'warning'
    case 'DELETE':
      return 'danger'
    default:
      return 'info'
  }
}

const updateRolePermission = async () => {
  if (loading.value) {
    return
  }

  const permissionIds = tableData.value
    .filter((item) => item['hasPermission'])
    .map((item) => item['permissionId'])

  loading.value = true

  try {
    await updateRolePermissionApi(props.role.id, permissionIds)
    ElMessage({
      message: t('el.common.role.updateRolePermissionSuccessful'),
      type: 'success',
    })
    closeDrawer()
  } catch (e) {
    ElMessage({
      message: t('el.common.role.updateRolePermissionFailed'),
      type: 'error',
    })
    console.error(e)
  } finally {
    loading.value = false
  }
}

const controllerFilters = computed(() => {
  if (tableData.value) {
    const controllers = tableData.value.map((item) => item['controllerName'])
    const uniqueControllers = new Set(controllers)
    let filter = []
    for (const uniqueController of uniqueControllers) {
      filter.push({ text: uniqueController, value: uniqueController })
    }
    return filter
  }
  return []
})

const handleControllerFilter = (value, row) => {
  return row['controllerName'] === value
}
</script>

<template>
  <ElDrawer
    :title="drawerTitle"
    :size="drawerSize"
    v-model="showDrawer"
    destroy-on-close
  >
    <div class="container">
      <div class="role-name">
        <span>{{ t('el.common.role.roleColon') }}</span>
        <span>{{ role['roleName'] }}</span>
      </div>
      <ElTable :data="tableData" stripe class="table" show-overflow-tooltip>
        <ElTableColumn width="40">
          <template #default="{ row }">
            <ElCheckbox v-model="row['hasPermission']"></ElCheckbox>
          </template>
        </ElTableColumn>
        <ElTableColumn
          :label="t('el.common.role.controller')"
          width="160"
          prop="controllerName"
          :filters="controllerFilters"
          :filter-method="handleControllerFilter"
        >
        </ElTableColumn>
        <ElTableColumn
          :label="t('el.common.role.method')"
          width="90"
          prop="httpMethod"
        >
          <template #default="{ row }">
            <!--suppress JSValidateTypes -->
            <ElTag
              size="small"
              effect="plain"
              :type="getMethodTagType(row['httpMethod'])"
            >
              {{ row['httpMethod'] }}
            </ElTag>
          </template>
        </ElTableColumn>
        <ElTableColumn
          :label="t('el.common.role.route')"
          min-width="320"
          prop="route"
        >
        </ElTableColumn>
        <ElTableColumn
          :label="t('el.common.role.description')"
          min-width="320"
          prop="actionDescription"
        >
        </ElTableColumn>
      </ElTable>
      <div class="footer">
        <ElButton
          type="primary"
          :loading="loading"
          @click="updateRolePermission"
        >
          {{ t('el.common.role.update') }}
        </ElButton>
      </div>
    </div>
  </ElDrawer>
</template>

<style scoped lang="scss">
.container {
  height: 100%;
  overflow: auto;
  display: flex;
  flex-flow: column;

  > .role-name {
    border-left: 4px solid var(--el-color-primary);
    padding: 1rem;
    background-color: var(--el-color-primary-light-9);
  }

  > .table {
    flex: 1;
    height: 100%;
    margin-top: 1rem;
  }

  > .footer {
    margin-top: 1rem;
  }
}
</style>
