<script setup>
import ContentLayout from '../layouts/ContentLayout.vue'
import { useLocale, dayjs } from 'element-plus'
import { useGetUser } from '../composables/getUser'
import AddUser from '../components/AddUser.vue'
import { ref } from 'vue'
import UpdateUser from '../components/UpdateUser.vue'
import DeleteUser from '../components/DeleteUser.vue'
import UpdateUserRole from '../components/UpdateUserRole.vue'

const { t } = useLocale()

const {
  loading,
  data,
  pageIndex,
  pageSize,
  handlePageChange,
  getUser,
  handlePageSizeChange,
} = useGetUser()

const showAddUserDrawer = ref(false)
const handleAddUser = () => {
  showAddUserDrawer.value = true
}

const currentUser = ref()

const showUpdateUserDrawer = ref(false)
const handleUpdateUser = (user) => {
  currentUser.value = JSON.parse(JSON.stringify(user))
  showUpdateUserDrawer.value = true
}

const showDeleteUserDrawer = ref(false)
const handleDeleteUser = (user) => {
  currentUser.value = JSON.parse(JSON.stringify(user))
  showDeleteUserDrawer.value = true
}

const showUpdateUserRoleDrawer = ref(false)
const handleUpdateUserRole = (user) => {
  currentUser.value = JSON.parse(JSON.stringify(user))
  showUpdateUserRoleDrawer.value = true
}

const handleEditFinish = async () => {
  currentUser.value = null
  await getUser()
}
</script>

<template>
  <ContentLayout v-loading="loading">
    <template #header>
      {{ t('el.common.user.userSetting') }}
    </template>
    <template #default>
      <div class="operation-container">
        <ElButton type="primary" plain @click="handleAddUser">
          {{ t('el.common.user.addUser') }}
        </ElButton>
      </div>
      <div class="data-container">
        <ElTable class="table" :data="data.items" stripe show-overflow-tooltip>
          <ElTableColumn
            :label="t('el.common.user.username')"
            prop="username"
            min-width="160"
          >
          </ElTableColumn>
          <ElTableColumn
            :label="t('el.common.user.fullName')"
            prop="fullName"
            min-width="160"
          >
          </ElTableColumn>
          <ElTableColumn
            :label="t('el.common.user.lastLogin')"
            prop="lastLogin"
            width="180"
          >
            <template #default="{ row }">
              <template v-if="row['lastLogin'] === 0">-</template>
              <template v-else>
                {{ dayjs.unix(row['lastLogin']).format('YYYY-MM-DD HH:mm:ss') }}
              </template>
            </template>
          </ElTableColumn>
          <ElTableColumn width="150">
            <template #default="{ row }">
              <ElButton
                type="primary"
                plain
                size="small"
                :disabled="row['username'].toLowerCase() === 'sysadmin'"
                @click="handleUpdateUserRole(row)"
              >
                {{ t('el.common.user.updateUserRole') }}
              </ElButton>
            </template>
          </ElTableColumn>
          <ElTableColumn width="150">
            <template #default="{ row }">
              <ElButtonGroup size="small">
                <ElButton type="primary" plain @click="handleUpdateUser(row)">
                  {{ t('el.common.user.update') }}
                </ElButton>
                <ElButton
                  type="danger"
                  plain
                  @click="handleDeleteUser(row)"
                  :disabled="row['username'].toLowerCase() === 'sysadmin'"
                >
                  {{ t('el.common.user.delete') }}
                </ElButton>
              </ElButtonGroup>
            </template>
          </ElTableColumn>
        </ElTable>
        <!--suppress JSValidateTypes -->
        <ElPagination
          layout="prev, pager, next, sizes, total"
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

  <AddUser v-model="showAddUserDrawer" @finish="handleEditFinish"></AddUser>
  <UpdateUser
    v-model="showUpdateUserDrawer"
    :user="currentUser"
    @finish="handleEditFinish"
  >
  </UpdateUser>
  <DeleteUser
    v-model="showDeleteUserDrawer"
    :user="currentUser"
    @finish="handleEditFinish"
  >
  </DeleteUser>
  <UpdateUserRole v-model="showUpdateUserRoleDrawer" :user="currentUser">
  </UpdateUserRole>
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
