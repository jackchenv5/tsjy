<script setup>
/*
 * 备注：
 * 2027-7-14
 * 连接器状态采用树形表格，而变量未采用树形表格的原因
 * 在一个应用中，连接器比较少，目前已有的仅有 S7, OPC UA, ModbusTCP 三个
 * 而连接一般不会超过 20 个，所以采用树形表格，一次加载所有数据
 *
 * 变量数据可能比较多，需要分页加载，所以采用普通表格
 * */
import ContentLayout from '#/common/layouts/ContentLayout.vue'
import { useGetDefinitions } from '../composables/getDefinitions'
import { onBeforeUnmount, ref } from 'vue'
import { dayjs, useLocale } from 'element-plus'
import { Refresh, Loading } from '@element-plus/icons-vue'

const { t } = useLocale()

const {
  loading,
  data,
  pageIndex,
  pageSize,
  nameFilter,
  getDefinitions,
  handlePageChange,
  handlePageSizeChange,
} = useGetDefinitions()

// 用于支持自定义表格列
// eslint-disable-next-line no-unused-vars
const columns = ref([
  {
    label: 'el.variable.variableMonitor.connectorInstance',
    prop: 'connectorInstance',
    width: 170,
  },
  {
    label: 'el.variable.variableMonitor.connectionName',
    prop: 'connectionName',
    minWidth: 180,
  },
  {
    label: 'el.variable.variableMonitor.dataPointName',
    prop: 'dataPointName',
    width: 150,
  },
  {
    label: 'el.variable.variableMonitor.name',
    prop: 'name',
    minWidth: 300,
  },
  {
    label: 'el.variable.variableMonitor.value',
    prop: 'val',
    minWidth: 150,
  },
  {
    label: 'el.variable.variableMonitor.dataType',
    prop: 'dataType',
    width: 100,
  },
  {
    label: 'el.variable.variableMonitor.accessMode',
    prop: 'accessMode',
    width: 120,
  },
  {
    label: 'el.variable.variableMonitor.acquisitionCycleInMs',
    prop: 'acquisitionCycleInMs',
    width: 200,
  },
  {
    label: 'el.variable.variableMonitor.acquisitionMode',
    prop: 'acquisitionMode',
    width: 200,
  },
  {
    label: 'el.variable.variableMonitor.id',
    prop: 'id',
    width: 70,
  },
  {
    label: 'el.variable.variableMonitor.isArray',
    prop: 'isArray',
    width: 80,
  },
  {
    label: 'el.variable.variableMonitor.qualityCode',
    prop: 'qc',
    width: 150,
  },

  {
    label: 'el.variable.variableMonitor.lastUpdated',
    prop: 'ts',
    width: 180,
  },
])

const displayColumns = ref([
  {
    label: 'el.variable.variableMonitor.connectorInstance',
    prop: 'connectorInstance',
    width: 170,
  },
  {
    label: 'el.variable.variableMonitor.connectionName',
    prop: 'connectionName',
    minWidth: 180,
  },
  {
    label: 'el.variable.variableMonitor.dataPointName',
    prop: 'dataPointName',
    width: 150,
  },
  {
    label: 'el.variable.variableMonitor.name',
    prop: 'name',
    minWidth: 300,
  },
  {
    label: 'el.variable.variableMonitor.value',
    prop: 'val',
    minWidth: 150,
  },
  {
    label: 'el.variable.variableMonitor.dataType',
    prop: 'dataType',
    width: 100,
  },
  {
    label: 'el.variable.variableMonitor.lastUpdated',
    prop: 'ts',
    width: 180,
  },
])

const getQcString = (qc) => {
  switch (qc) {
    case 0:
      return {
        text: 'Bad',
        type: 'danger',
      }
    case 1:
      return {
        text: 'Uncertain',
        type: 'warning',
      }
    case 2:
      return {
        text: 'GoodNonCascade',
        type: 'primary',
      }
    case 3:
      return {
        text: 'GoodCascade',
        type: 'success',
      }
    default:
      return {
        text: 'Unknown',
        type: 'info',
      }
  }
}

const handleRefresh = async () => {
  await getDefinitions()
}

const autoFresh = ref(false)
let interval
const handleAutoRefreshChange = async () => {
  if (interval) {
    clearInterval(interval)
  }
  if (autoFresh.value) {
    interval = setInterval(async () => {
      await getDefinitions()
    }, 2000)
  }
}

onBeforeUnmount(() => {
  if (interval) {
    clearInterval(interval)
  }
})
</script>

<template>
  <ContentLayout>
    <template #header>
      {{ t('el.variable.variableMonitor.variableMonitor') }}
    </template>
    <template #default>
      <div class="operation-container">
        <ElInput
          class="filter-item"
          v-model="nameFilter"
          :placeholder="t('el.variable.variableMonitor.nameFilterPlaceholder')"
          clearable
          :disabled="autoFresh"
        >
        </ElInput>
        <ElButton
          type="primary"
          plain
          :icon="Refresh"
          @click="handleRefresh"
          :disabled="autoFresh || loading"
        >
          {{ t('el.variable.variableMonitor.refresh') }}
        </ElButton>
        <ElCheckbox
          :label="t('el.variable.variableMonitor.autoRefresh')"
          v-model="autoFresh"
          @change="handleAutoRefreshChange"
        >
        </ElCheckbox>
        <div class="loading-icon">
          <ElIcon v-if="loading" class="is-loading">
            <Loading></Loading>
          </ElIcon>
        </div>
      </div>
      <div class="data-container" v-loading="loading">
        <ElTable
          class="table"
          stripe
          :data="data['items']"
          show-overflow-tooltip
        >
          <ElTableColumn
            v-for="(col, index) of displayColumns"
            :key="index"
            :label="t(col['label'])"
            :prop="col['prop']"
            :width="col['width']"
            :min-width="col['minWidth']"
          >
            <!-- eslint-disable-next-line vue/no-unused-vars -->
            <template #header="scope"></template>
            <template #default="scope">
              <template v-if="col['prop'] === 'accessMode'">
                <ElTag
                  :type="scope.row['accessMode'] === 'rw' ? 'primary' : 'info'"
                  size="small"
                >
                  {{ scope.row['accessMode'] }}
                </ElTag>
              </template>
              <template v-else-if="col['prop'] === 'qc'">
                <ElTag
                  :type="getQcString(scope.row['qc'])['type']"
                  effect="dark"
                  size="small"
                >
                  {{ getQcString(scope.row['qc'])['text'] }}
                </ElTag>
              </template>
              <template v-else-if="col['prop'] === 'ts'">
                <ElTag v-if="scope.row['ts']" type="info" size="small">
                  {{ dayjs(scope.row['ts']).format('YYYY-MM-DD HH:mm:ss') }}
                </ElTag>
              </template>
              <template v-else-if="col['prop'] === 'dataType'">
                <template v-if="scope.row['isArray']">
                  {{ `<${scope.row['dataType']}>` }}
                </template>
                <template v-else>
                  {{ `${scope.row['dataType']}` }}
                </template>
              </template>
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
          :page-sizes="[20, 50, 100]"
          @update:page-size="handlePageSizeChange"
          :disabled="loading || autoFresh"
        >
        </ElPagination>
      </div>
    </template>
  </ContentLayout>
</template>

<style scoped lang="scss">
.operation-container {
  margin-bottom: 1rem;
  display: flex;
  flex-flow: row wrap;
  grid-gap: 1rem;

  > .filter-item {
    width: 20rem;
  }

  > .loading-icon {
    display: flex;
    align-items: center;
    color: var(--el-color-primary);
    font-size: 2rem;
  }
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
