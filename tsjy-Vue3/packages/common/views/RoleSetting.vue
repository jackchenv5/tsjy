<script setup>
import ContentLayout from '../layouts/ContentLayout.vue'
import { useLocale } from 'element-plus'
import { useGetRole } from '../composables/getRole'
import { ref } from 'vue'
import AddRole from '../components/AddRole.vue'
import UpdateRole from '../components/UpdateRole.vue'
import DeleteRole from '../components/DeleteRole.vue'
import RolePermission from '../components/RolePermission.vue'
import RoleMenu from '../components/RoleMenu.vue'

const { t } = useLocale()

const {
  loading,
  data,
  pageIndex,
  pageSize,
  handlePageChange,
  getRole,
  handlePageSizeChange,
} = useGetRole()

const showAddRoleDrawer = ref(false)
const handleAddRole = () => {
  showAddRoleDrawer.value = true
}

const currentRole = ref()

const showUpdateRoleDrawer = ref(false)
const handleUpdateRole = (role) => {
  currentRole.value = JSON.parse(JSON.stringify(role))
  showUpdateRoleDrawer.value = true
}

const showDeleteRoleDrawer = ref(false)
const handleDeleteRole = (role) => {
  currentRole.value = JSON.parse(JSON.stringify(role))
  showDeleteRoleDrawer.value = true
}

const handleEditFinish = async () => {
  await getRole()
}

const showRolePermissionDrawer = ref(false)
const handleRolePermission = (role) => {
  currentRole.value = JSON.parse(JSON.stringify(role))
  showRolePermissionDrawer.value = true
}

const showRoleMenu = ref(false)
const handleRoleMenu = (role) => {
  currentRole.value = JSON.parse(JSON.stringify(role))
  showRoleMenu.value = true
}
</script>

<template>
  <ContentLayout v-loading="loading">
    <template #header>
      {{ t('el.common.role.roleSetting') }}
    </template>
    <template #default>
      <div class="operation-container">
        <ElButton type="primary" plain @click="handleAddRole">
          {{ t('el.common.role.addRole') }}
        </ElButton>
      </div>
      <div class="data-container">
        <ElTable :data="data.items" stripe class="table">
          <ElTableColumn
            :label="t('el.common.role.roleName')"
            prop="roleName"
            min-width="160"
          >
          </ElTableColumn>
          <ElTableColumn width="270">
            <template #default="{ row }">
              <ElButtonGroup size="small">
                <ElButton
                  type="primary"
                  plain
                  :disabled="row['roleName'].toLowerCase() === 'sysadmin'"
                  @click="handleRoleMenu(row)"
                >
                  {{ t('el.common.role.roleMenu') }}
                </ElButton>
                <ElButton
                  type="primary"
                  plain
                  :disabled="row['roleName'].toLowerCase() === 'sysadmin'"
                  @click="handleRolePermission(row)"
                >
                  {{ t('el.common.role.rolePermission') }}
                </ElButton>
              </ElButtonGroup>
            </template>
          </ElTableColumn>
          <ElTableColumn width="150">
            <template #default="{ row }">
              <ElButtonGroup size="small">
                <ElButton
                  type="primary"
                  plain
                  :disabled="row['roleName'].toLowerCase() === 'sysadmin'"
                  @click="handleUpdateRole(row)"
                >
                  {{ t('el.common.role.update') }}
                </ElButton>
                <ElButton
                  type="danger"
                  plain
                  :disabled="row['roleName'].toLowerCase() === 'sysadmin'"
                  @click="handleDeleteRole(row)"
                >
                  {{ t('el.common.role.delete') }}
                </ElButton>
              </ElButtonGroup>
            </template>
          </ElTableColumn>
        </ElTable>
        <!--suppress JSValidateTypes -->
        <ElPagination
          layout="prev, pager, next, sizes"
          :total="data.total"
          :default-page-size="20"
          :pager-count="5"
          v-model:current-page="pageIndex"
          @update:current-page="handlePageChange"
          v-model:page-size="pageSize"
          :page-sizes="[20, 50, 100]"
          @update:page-size="handlePageSizeChange"
          :disabled="loading"
        >
        </ElPagination>
      </div>
    </template>
  </ContentLayout>

  <AddRole v-model="showAddRoleDrawer" @finish="handleEditFinish"></AddRole>
  <UpdateRole
    v-model="showUpdateRoleDrawer"
    @finish="handleEditFinish"
    :role="currentRole"
  >
  </UpdateRole>
  <DeleteRole
    v-model="showDeleteRoleDrawer"
    @finish="handleEditFinish"
    :role="currentRole"
  >
  </DeleteRole>
  <RolePermission v-model="showRolePermissionDrawer" :role="currentRole">
  </RolePermission>
  <RoleMenu v-model="showRoleMenu" :role="currentRole"></RoleMenu>
</template>

<style scoped lang="scss">
.operation-container {
  margin-bottom: 1rem;
}

.data-container {
  display: flex;
  height: 100%;
  flex-flow: column;
  overflow: auto;

  > .table {
    flex: 1;
    margin-bottom: 1rem;
  }
}
</style>
