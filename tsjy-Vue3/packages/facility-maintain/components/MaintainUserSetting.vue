<script setup>
import { useLocale } from 'element-plus'
import { onMounted, ref } from 'vue'
import SelectUser from '#/common/components/SelectUser.vue'
import SelectRole from '#/common/components/SelectRole.vue'
import {
  addMaintainRolesApi,
  addMaintainUsersApi,
  getMaintainRolesApi,
  getMaintainUsersApi,
  removeMaintainRoleApi,
  removeMaintainUserApi,
} from '../apis/maintainUser'

// region 组件
const { t } = useLocale()
onMounted(async () => {
  await getMaintainUsers()
  await getMaintainRoles()
})
// endregion

// region 用户数据
const showSelectUser = ref(false)
const selectUser = () => {
  showSelectUser.value = true
}
const selectedUser = ref()
const onSelectUser = async (user) => {
  selectedUser.value = user
  await addMaintainUser()
}
//endregion

// region 维护用户
const loadingAddMaintainUser = ref(false)
const addMaintainUser = async () => {
  loadingAddMaintainUser.value = true
  try {
    await addMaintainUsersApi([selectedUser.value.id])
    await getMaintainUsers()
  } catch (e) {
    console.error(e)
  } finally {
    loadingAddMaintainUser.value = false
  }
}

const maintainUsers = ref([])
const loadingGetMaintainUsers = ref(false)
const getMaintainUsers = async () => {
  loadingGetMaintainUsers.value = true
  try {
    const { data } = await getMaintainUsersApi()
    maintainUsers.value = data
  } catch (e) {
    console.error(e)
  } finally {
    loadingGetMaintainUsers.value = false
  }
}

const loadingRemoveMaintainUser = ref(false)
const removeMaintainUser = async (row) => {
  loadingRemoveMaintainUser.value = true
  try {
    await removeMaintainUserApi(row.id)
    await getMaintainUsers()
  } catch (e) {
    console.error(e)
  } finally {
    loadingRemoveMaintainUser.value = false
  }
}
// endregion

// region 角色数据
const showSelectRole = ref(false)
const selectRole = () => {
  showSelectRole.value = true
}
const selectedRole = ref()
const onSelectRole = async (role) => {
  selectedRole.value = role
  await addMaintainRole()
}
//endregion

// region 维护角色
const loadingAddMaintainRole = ref(false)
const addMaintainRole = async () => {
  loadingAddMaintainRole.value = true
  try {
    await addMaintainRolesApi([selectedRole.value.id])
    await getMaintainRoles()
  } catch (e) {
    console.error(e)
  } finally {
    loadingAddMaintainRole.value = false
  }
}

const maintainRoles = ref([])
const loadingGetMaintainRoles = ref(false)
const getMaintainRoles = async () => {
  loadingGetMaintainRoles.value = true
  try {
    const { data } = await getMaintainRolesApi()
    maintainRoles.value = data
  } catch (e) {
    console.error(e)
  } finally {
    loadingGetMaintainRoles.value = false
  }
}

const loadingRemoveMaintainRole = ref(false)
const removeMaintainRole = async (row) => {
  loadingRemoveMaintainRole.value = true
  try {
    await removeMaintainRoleApi(row.id)
    await getMaintainRoles()
  } catch (e) {
    console.error(e)
  } finally {
    loadingRemoveMaintainRole.value = false
  }
}
//endregion
</script>

<template>
  <div class="maintain-user-setting-root">
    <div class="maintain-user-container">
      <ElTable class="table" :data="maintainUsers">
        <ElTableColumn
          :label="t('el.facilityMaintain.maintainSetting.maintainUser')"
          width="160"
          prop="user.username"
        >
        </ElTableColumn>
        <ElTableColumn
          :label="t('el.facilityMaintain.maintainSetting.fullName')"
          width="160"
          prop="user.fullName"
        >
        </ElTableColumn>
        <ElTableColumn width="100">
          <template #default="{ row }">
            <ElPopconfirm
              @confirm="removeMaintainUser(row)"
              :title="t('el.facilityMaintain.maintainSetting.removeConfirm')"
            >
              <template #reference>
                <ElButton type="danger" size="small">
                  {{ t('el.facilityMaintain.maintainSetting.remove') }}
                </ElButton>
              </template>
            </ElPopconfirm>
          </template>
        </ElTableColumn>
      </ElTable>
      <div>
        <ElButton type="primary" plain @click="selectUser">
          {{ t('el.facilityMaintain.maintainSetting.selectUser') }}
        </ElButton>
      </div>
    </div>
    <div class="maintain-role-container">
      <ElTable class="table" :data="maintainRoles">
        <ElTableColumn
          :label="t('el.facilityMaintain.maintainSetting.maintainRole')"
          width="160"
          prop="role.roleName"
        >
        </ElTableColumn>
        <ElTableColumn width="100">
          <template #default="{ row }">
            <ElPopconfirm
              @confirm="removeMaintainRole(row)"
              :title="t('el.facilityMaintain.maintainSetting.removeConfirm')"
            >
              <template #reference>
                <ElButton type="danger" size="small">
                  {{ t('el.facilityMaintain.maintainSetting.remove') }}
                </ElButton>
              </template>
            </ElPopconfirm>
          </template>
        </ElTableColumn>
      </ElTable>
      <div>
        <ElButton type="primary" plain @click="selectRole">
          {{ t('el.facilityMaintain.maintainSetting.selectRole') }}
        </ElButton>
      </div>
    </div>
  </div>

  <SelectUser v-model="showSelectUser" @select="onSelectUser"></SelectUser>
  <SelectRole v-model="showSelectRole" @select="onSelectRole"></SelectRole>
</template>

<style scoped lang="scss">
.maintain-user-setting-root {
  flex: 1;
  display: flex;
  overflow: auto;
  grid-gap: 1rem;

  > .maintain-user-container {
    display: flex;
    flex-flow: column;
    grid-gap: 1rem;
    overflow: auto;

    > .table {
      flex: 1;

      .full-name {
        font-size: var(--el-font-size-small);
        color: var(--el-text-color-secondary);
      }
    }
  }

  > .maintain-role-container {
    display: flex;
    flex-flow: column;
    grid-gap: 1rem;
    overflow: auto;

    > .table {
      flex: 1;
    }
  }
}
</style>
