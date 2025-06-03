<script setup>
import { ElMessage, useLocale } from 'element-plus'
import { ref, watch } from 'vue'
import { getUserRoleApi, updateUserRoleApi } from '../apis/userRole'
import { useDrawer } from '../composables/drawer'

const props = defineProps(['modelValue', 'user'])
const emits = defineEmits(['update:modelValue'])

const { showDrawer, drawerTitle, drawerSize, openDrawer, closeDrawer } =
  useDrawer()

const { t } = useLocale()

watch(
  () => props.modelValue,
  async (val) => {
    if (val) {
      drawerTitle.value = t('el.common.user.updateUserRole')
      openDrawer('400')
      await getUserRole()
    }
  },
)

watch(showDrawer, (val) => {
  emits('update:modelValue', val)
})

const loading = ref(false)

const tableData = ref([])

const getUserRole = async () => {
  if (loading.value) {
    return
  }

  loading.value = true

  try {
    const res = await getUserRoleApi(props.user.id)
    tableData.value = res.data
  } catch (e) {
    console.error(e)
  } finally {
    loading.value = false
  }
}

const updateUserRole = async () => {
  if (loading.value) {
    return
  }

  const roleIds = tableData.value
    .filter((item) => item['hasRole'])
    .map((item) => item['roleId'])

  loading.value = true

  try {
    await updateUserRoleApi(props.user.id, roleIds)
    ElMessage({
      message: t('el.common.user.userRolesUpdateSuccessful'),
      type: 'success',
    })
    closeDrawer()
  } catch (e) {
    ElMessage({
      message: t('el.common.user.userRolesUpdateFailed'),
      type: 'error',
    })
    console.error(e)
  } finally {
    loading.value = false
  }
}

const handleUpdateUserRole = async () => {
  await updateUserRole()
}
</script>

<template>
  <ElDrawer :title="drawerTitle" v-model="showDrawer" :size="drawerSize">
    <div class="container">
      <div class="user-info">
        <span>{{ t('el.common.user.userColon') }}</span>
        <span>{{ user['username'] }}</span>
      </div>
      <ElTable :data="tableData" stripe class="table">
        <ElTableColumn width="40">
          <template #default="{ row }">
            <ElCheckbox v-model="row['hasRole']"></ElCheckbox>
          </template>
        </ElTableColumn>
        <ElTableColumn :label="t('el.common.user.roleName')" prop="roleName">
        </ElTableColumn>
      </ElTable>
      <div class="footer">
        <ElButton
          type="primary"
          :loading="loading"
          @click="handleUpdateUserRole"
        >
          {{ t('el.common.user.update') }}
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

  > .user-info {
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
