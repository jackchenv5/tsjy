<script setup>
import ContentLayout from '#/common/layouts/ContentLayout.vue'
import { onMounted, ref } from 'vue'
import {
  addArchivedVariableApi,
  getArchivedVariablesApi,
} from '#/variable/apis/variableArchive'
import { ElMessage, useLocale } from 'element-plus'
import { ArrowDown, Edit } from '@element-plus/icons-vue'
import VariableFilter from '../components/VariableFilter.vue'
import UpdateArchive from '../components/UpdateArchive.vue'
import DeleteArchive from '../components/DeleteArchive.vue'
import SelectVariable from '../components/SelectVariable.vue'

const data = ref({
  total: 0,
  items: [],
})

const pageIndex = ref(1)
const pageSize = ref(20)
const handlePageChange = async () => {
  await getArchiveVariables()
}
const handlePageSizeChange = async () => {
  await getArchiveVariables()
}

const loading = ref(false)
const getArchiveVariables = async () => {
  if (loading.value) {
    return
  }

  loading.value = true

  try {
    const filter = {
      name: nameFilter.value,
      pageIndex: pageIndex.value,
      pageSize: pageSize.value,
    }
    const res = await getArchivedVariablesApi(filter)
    data.value = res.data
  } catch (e) {
    console.error(e)
  } finally {
    loading.value = false
  }
}

onMounted(async () => {
  await getArchiveVariables()
})

const { t } = useLocale()

const nameFilter = ref('')
const handleFilter = async (filters) => {
  nameFilter.value = filters.name

  await getArchiveVariables()
}

const handleEditFinish = async () => {
  currentArchiveVariable.value = null
  await getArchiveVariables()
}

const updateArchive = ref(false)
const currentArchiveVariable = ref()
const handleUpdateArchive = (row) => {
  currentArchiveVariable.value = JSON.parse(JSON.stringify(row))
  updateArchive.value = true
}

const deleteArchive = ref(false)
const onDeleteArchive = (row) => {
  currentArchiveVariable.value = JSON.parse(JSON.stringify(row))
  deleteArchive.value = true
}

// region 添加归档变量

/*
 * 归档模式
 * 0 - 变化时归档
 * 1 - 周期归档
 * */
const archiveMode = ref(0)
const onAddArchive = (command) => {
  archiveMode.value = command

  addArchive.value = true
}

const addArchive = ref(false)
const loadingAddArchivedVariable = ref(false)
const onVariableSelect = async (value) => {
  loadingAddArchivedVariable.value = true
  try {
    await addArchivedVariableApi(value['guid'], archiveMode.value)
    ElMessage({
      message: t('el.variable.variableArchive.archivedVariableAddSuccessful'),
      type: 'success',
    })
    await getArchiveVariables()
  } catch (e) {
    ElMessage({
      message: t('el.variable.variableArchive.archivedVariableAddFailed'),
      type: 'error',
    })
    console.error(e)
  } finally {
    loadingAddArchivedVariable.value = false
  }
}
// endregion
</script>

<template>
  <ContentLayout>
    <template #header>
      {{ t('el.variable.variableArchive.variableArchive') }}
    </template>
    <template #default>
      <div class="operation-container">
        <ElDropdown @command="onAddArchive">
          <ElButton type="primary" plain>
            <span>
              {{ t('el.variable.variableArchive.addArchive') }}
            </span>
            <ElIcon class="el-icon--right">
              <ArrowDown></ArrowDown>
            </ElIcon>
          </ElButton>
          <template #dropdown>
            <ElDropdownMenu>
              <ElDropdownItem :command="0">
                {{ t('el.variable.variableArchive.archiveOnChanged') }}
              </ElDropdownItem>
              <ElDropdownItem :command="1">
                {{ t('el.variable.variableArchive.archiveOnInterval') }}
              </ElDropdownItem>
            </ElDropdownMenu>
          </template>
        </ElDropdown>

        <div class="divider"></div>
        <VariableFilter @filter="handleFilter" :loading="loading">
        </VariableFilter>
      </div>
      <div class="data-container">
        <ElTable
          stripe
          show-overflow-tooltip
          :data="data['items']"
          class="table"
          v-loading="loading"
        >
          <ElTableColumn
            :label="t('el.variable.variableArchive.connectorInstance')"
            prop="connectorInstance"
            width="170"
          >
          </ElTableColumn>
          <ElTableColumn
            :label="t('el.variable.variableArchive.connectionName')"
            prop="connectionName"
            width="180"
          >
          </ElTableColumn>
          <ElTableColumn
            :label="t('el.variable.variableArchive.dataPointName')"
            prop="dataPoint"
            width="150"
          >
          </ElTableColumn>
          <ElTableColumn
            :label="t('el.variable.variableArchive.name')"
            prop="name"
            min-width="220"
          >
          </ElTableColumn>
          <ElTableColumn
            :label="t('el.variable.variableArchive.archiveType')"
            prop="archiveType"
            width="130"
          >
            <template #default="{ row }">
              <ElTag
                v-if="row['archiveType'] === 0"
                type="warning"
                size="small"
              >
                {{ t('el.variable.variableArchive.systemArchiveType') }}
              </ElTag>
              <ElTag
                v-else-if="row['archiveType'] === 1"
                type="primary"
                size="small"
              >
                {{ t('el.variable.variableArchive.userArchiveType') }}
              </ElTag>
            </template>
          </ElTableColumn>
          <ElTableColumn
            :label="t('el.variable.variableArchive.archiveMode')"
            prop="archiveMode"
            width="130"
          >
            <template #default="{ row }">
              <ElTag
                v-if="row['archiveMode'] === 0"
                type="primary"
                size="small"
              >
                {{ t('el.variable.variableArchive.archiveOnChanged') }}
              </ElTag>
              <ElTag
                v-else-if="row['archiveMode'] === 1"
                type="warning"
                size="small"
              >
                {{ t('el.variable.variableArchive.archiveOnInterval') }}
              </ElTag>
            </template>
          </ElTableColumn>
          <ElTableColumn
            :label="t('el.variable.variableArchive.archiveInterval')"
            prop="archiveInterval"
            width="140"
          >
            <template #default="{ row }">
              <ElTag v-if="row['archiveMode'] === 0" type="info" size="small">
                {{ t('el.variable.variableArchive.notApplicable') }}
              </ElTag>
              <div v-else>
                <ElTag type="success" size="small" effect="light">
                  {{
                    `${row['archiveInterval']} ${t('el.variable.variableArchive.archiveIntervalUnit')}`
                  }}
                </ElTag>
                <ElButton
                  type="primary"
                  link
                  @click="handleUpdateArchive(row)"
                  :icon="Edit"
                >
                </ElButton>
              </div>
            </template>
          </ElTableColumn>
          <ElTableColumn width="90">
            <template #default="{ row }">
              <ElButton
                type="danger"
                plain
                @click="onDeleteArchive(row)"
                size="small"
                :disabled="row['archiveType'] !== 1"
              >
                {{ t('el.variable.variableArchive.delete') }}
              </ElButton>
            </template>
          </ElTableColumn>
        </ElTable>
        <ElPagination
          layout="prev, pager, next, sizes, total"
          :total="data.total"
          :default-page-size="20"
          :pager-count="5"
          v-model:current-page="pageIndex"
          @update:current-page="handlePageChange"
          v-model:page-size="pageSize"
          :page-sizes="[10, 20, 50, 100]"
          @update:page-size="handlePageSizeChange"
          :disabled="loading"
        >
        </ElPagination>
      </div>
    </template>
  </ContentLayout>

  <SelectVariable v-model="addArchive" @select="onVariableSelect">
  </SelectVariable>
  <UpdateArchive
    v-model="updateArchive"
    :archived-variable="currentArchiveVariable"
    @finish="handleEditFinish"
  >
  </UpdateArchive>
  <DeleteArchive
    v-model="deleteArchive"
    :archived-variable="currentArchiveVariable"
    @finish="handleEditFinish"
  >
  </DeleteArchive>
</template>

<style scoped lang="scss">
.operation-container {
  margin-bottom: 1rem;
  display: flex;
  flex-flow: row wrap;
  grid-gap: 1rem;
  align-items: center;

  > .divider {
    height: 2.4rem;
    width: 1px;
    background-color: var(--el-border-color);
  }
}

.data-container {
  display: flex;
  height: 100%;
  flex-flow: column;
  overflow: auto;
  border-top: 1px solid var(--el-border-color);

  > .table {
    flex: 1;
    margin-bottom: 1rem;
  }
}
</style>
