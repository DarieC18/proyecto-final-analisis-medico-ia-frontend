<template>
  <div class="container">
    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-primary" role="status"></div>
    </div>

    <div v-else-if="error" class="alert alert-danger border-0 rounded-3">{{ error }}</div>

    <template v-else-if="cita">
      <div class="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h3 class="fw-bold mb-0">Expediente Clínico: {{ patientName }}</h3>
          <p class="text-muted">Cita #{{ cita.id }} - <StatusBadge :text="cita.status" :variant="statusVariant(cita.status)" /></p>
        </div>
        <RouterLink to="/citas" class="btn btn-light border text-muted shadow-sm">Volver al listado</RouterLink>
      </div>

      <div class="row g-3 mb-4">
        <div class="col-md-3">
          <div class="card bg-light border-0 shadow-sm p-3 text-center rounded-4">
            <small class="text-muted fw-bold text-uppercase">Fecha</small>
            <p class="fw-bold mb-0 mt-1">{{ formatDateTime(cita.appointmentDate) }}</p>
          </div>
        </div>
        <div class="col-md-3">
          <div class="card bg-light border-0 shadow-sm p-3 text-center rounded-4">
            <small class="text-muted fw-bold text-uppercase">Motivo</small>
            <p class="fw-bold mb-0 mt-1 small">{{ cita.reason }}</p>
          </div>
        </div>
        <div class="col-md-3">
          <div class="card bg-light border-0 shadow-sm p-3 text-center rounded-4">
            <small class="text-muted fw-bold text-uppercase">Paciente</small>
            <p class="fw-bold mb-0 mt-1 small">{{ patientName }}</p>
          </div>
        </div>
        <div class="col-md-3">
          <div class="card bg-light border-0 shadow-sm p-3 text-center rounded-4">
            <small class="text-muted fw-bold text-uppercase">Estado</small>
            <div class="mt-1">
              <select v-model="nuevoEstado" @change="cambiarEstado" class="form-select form-select-sm bg-white border-0 fw-bold text-center">
                <option value="Pending">Pendiente</option>
                <option value="InProgress">En Progreso</option>
                <option value="Completed">Completada</option>
                <option value="Cancelled">Cancelada</option>
              </select>
            </div>
          </div>
        </div>
      </div>

      <div class="card shadow-sm p-2 mb-4">
        <ul class="nav nav-pills nav-fill gap-2 p-1">
          <li class="nav-item">
            <a class="nav-link rounded-pill fw-medium" :class="{ 'active bg-primary shadow-sm': tabActual === 'info', 'text-muted': tabActual !== 'info' }" @click="tabActual = 'info'" href="#">📝 Info Básica</a>
          </li>
          <li class="nav-item">
            <a class="nav-link rounded-pill fw-medium" :class="{ 'active bg-primary shadow-sm': tabActual === 'sintomas', 'text-muted': tabActual !== 'sintomas' }" @click="tabActual = 'sintomas'" href="#">🤒 Síntomas</a>
          </li>
          <li class="nav-item">
            <a class="nav-link rounded-pill fw-medium" :class="{ 'active bg-primary shadow-sm': tabActual === 'signos', 'text-muted': tabActual !== 'signos' }" @click="tabActual = 'signos'" href="#">❤️ Signos Vitales</a>
          </li>
          <li class="nav-item">
            <a class="nav-link rounded-pill fw-bold" :class="{ 'active bg-dark text-white shadow-sm': tabActual === 'ia', 'text-primary bg-primary bg-opacity-10': tabActual !== 'ia' }" @click="tabActual = 'ia'" href="#">🤖 Análisis IA</a>
          </li>
          <li class="nav-item">
            <a class="nav-link rounded-pill fw-medium" :class="{ 'active bg-primary shadow-sm': tabActual === 'records', 'text-muted': tabActual !== 'records' }" @click="tabActual = 'records'" href="#">📋 Historial Médico</a>
          </li>
          <li class="nav-item">
            <a class="nav-link rounded-pill fw-medium" :class="{ 'active bg-primary shadow-sm': tabActual === 'alertas', 'text-muted': tabActual !== 'alertas' }" @click="tabActual = 'alertas'" href="#">🔔 Alertas</a>
          </li>
          <li class="nav-item">
            <a class="nav-link rounded-pill fw-medium" :class="{ 'active bg-primary shadow-sm': tabActual === 'recomendaciones', 'text-muted': tabActual !== 'recomendaciones' }" @click="tabActual = 'recomendaciones'" href="#">💡 Recomendaciones</a>
          </li>
          <li class="nav-item">
            <a class="nav-link rounded-pill fw-medium" :class="{ 'active bg-primary shadow-sm': tabActual === 'documentos', 'text-muted': tabActual !== 'documentos' }" @click="tabActual = 'documentos'" href="#">📄 Documentos</a>
          </li>
        </ul>
      </div>

      <div class="card shadow-sm">
        <div class="card-body p-5">

          <!-- INFO TAB -->
          <div v-if="tabActual === 'info'" class="animation-fade">
            <h5 class="fw-bold text-dark mb-4">Notas y Registro Clínico</h5>

            <div class="mb-4">
              <label class="form-label text-muted small fw-bold text-uppercase">Notas de la Cita</label>
              <textarea v-model="notasEdit" class="form-control bg-light border-0 p-4 rounded-4 mb-3" rows="4" placeholder="Redacte las notas de la consulta aquí..."></textarea>
              <div class="d-flex justify-content-end">
                <button @click="guardarNotas" class="btn btn-primary px-4 py-2" :disabled="!notasEdit || guardandoNotas">
                  <span v-if="guardandoNotas" class="spinner-border spinner-border-sm me-2"></span>
                  Guardar Notas
                </button>
              </div>
              <div v-if="notasGuardadas" class="alert alert-success border-0 rounded-3 mt-3 small">Notas guardadas exitosamente.</div>
            </div>

            <hr class="my-4">

            <h5 class="fw-bold text-dark mb-4">Registros Médicos</h5>
            <div v-if="cita.medicalRecords?.length">
              <div v-for="r in cita.medicalRecords" :key="r.id" class="card bg-light border-0 p-4 rounded-4 mb-3">
                <div class="d-flex justify-content-between mb-2">
                  <small class="text-muted fw-bold">{{ formatDate(r.createdAt) }}</small>
                  <small class="text-muted">Dr. #{{ r.createdByUserId }}</small>
                </div>
                <p class="mb-1"><strong>Diagnóstico Inicial:</strong> {{ r.diagnosisInitial || 'N/A' }}</p>
                <p class="mb-1"><strong>Notas:</strong> {{ r.notes || 'N/A' }}</p>
                <p v-if="r.antecedentes" class="mb-1"><strong>Antecedentes:</strong> {{ r.antecedentes }}</p>
                <p v-if="r.observacionesConsulta" class="mb-0"><strong>Observaciones:</strong> {{ r.observacionesConsulta }}</p>
              </div>
            </div>
            <div v-else class="text-muted text-center py-4">
              <p>No hay registros médicos para esta cita.</p>
            </div>
          </div>

          <!-- SINTOMAS TAB -->
          <div v-if="tabActual === 'sintomas'" class="animation-fade">
            <div class="d-flex justify-content-between align-items-center mb-4">
              <h5 class="fw-bold text-dark mb-0">Registro de Síntomas</h5>
              <button @click="toggleFormSintoma" class="btn btn-primary btn-sm px-3 shadow-sm">
                {{ showSintomaForm ? 'Cancelar' : '+ Agregar Síntoma' }}
              </button>
            </div>

            <div v-if="showSintomaForm" class="card bg-light border-0 p-4 rounded-4 mb-4">
              <div class="row g-3">
                <div class="col-md-4">
                  <label class="form-label text-muted small fw-bold text-uppercase">Síntoma</label>
                  <input v-model="sintomaForm.name" type="text" class="form-control bg-white border-0" placeholder="Ej: Fiebre">
                </div>
                <div class="col-md-3">
                  <label class="form-label text-muted small fw-bold text-uppercase">Severidad</label>
                  <select v-model="sintomaForm.severity" class="form-select bg-white border-0">
                    <option value="Leve">Leve</option>
                    <option value="Moderado">Moderado</option>
                    <option value="Severo">Severo</option>
                  </select>
                </div>
                <div class="col-md-3">
                  <label class="form-label text-muted small fw-bold text-uppercase">Inicio</label>
                  <input v-model="sintomaForm.startedAt" type="date" class="form-control bg-white border-0">
                </div>
                <div class="col-md-2 d-flex align-items-end">
                  <button @click="guardarSintoma" class="btn btn-success w-100" :disabled="guardandoSintoma">
                    <span v-if="guardandoSintoma" class="spinner-border spinner-border-sm"></span>
                    <span v-else>Agregar</span>
                  </button>
                </div>
              </div>
              <div class="mt-3">
                <label class="form-label text-muted small fw-bold text-uppercase">Notas</label>
                <textarea v-model="sintomaForm.notes" class="form-control bg-white border-0" rows="2" placeholder="Notas adicionales..."></textarea>
              </div>
            </div>

            <div v-if="sintomas.length === 0" class="text-muted text-center py-4">
              <p>No hay síntomas registrados para esta cita.</p>
            </div>
            <div class="row g-4">
              <div class="col-md-6" v-for="(s, idx) in sintomas" :key="s.id || idx">
                <div class="card bg-light border-0 border-start border-4 shadow-sm h-100 p-3 rounded-4" :class="`border-${severityColor(s.severity)}`">
                  <div class="d-flex justify-content-between align-items-start mb-2">
                    <h6 class="fw-bold mb-0">{{ s.name }}</h6>
                    <span class="badge rounded-pill px-3" :class="severityBadge(s.severity)">{{ s.severity }}</span>
                  </div>
                  <p class="text-muted small mb-2"><strong>Inicio:</strong> {{ formatDate(s.startedAt) }}</p>
                  <p class="mb-2 text-dark small">{{ s.notes || 'Sin notas adicionales.' }}</p>
                  <div class="d-flex justify-content-end gap-2 mt-2">
                    <button @click="editarSintoma(s)" class="btn btn-sm btn-light border text-warning px-3">Editar</button>
                    <button @click="confirmarEliminarSintoma(s)" class="btn btn-sm btn-light border text-danger px-3">Eliminar</button>
                  </div>
                </div>
              </div>
            </div>

            <div v-if="editSintomaId" class="card bg-light border-0 p-4 rounded-4 mt-4">
              <h6 class="fw-bold mb-3">Editar Síntoma</h6>
              <div class="row g-3">
                <div class="col-md-4">
                  <label class="form-label text-muted small fw-bold text-uppercase">Síntoma</label>
                  <input v-model="editSintomaForm.name" type="text" class="form-control bg-white border-0">
                </div>
                <div class="col-md-3">
                  <label class="form-label text-muted small fw-bold text-uppercase">Severidad</label>
                  <select v-model="editSintomaForm.severity" class="form-select bg-white border-0">
                    <option value="Leve">Leve</option>
                    <option value="Moderado">Moderado</option>
                    <option value="Severo">Severo</option>
                  </select>
                </div>
                <div class="col-md-3">
                  <label class="form-label text-muted small fw-bold text-uppercase">Inicio</label>
                  <input v-model="editSintomaForm.startedAt" type="date" class="form-control bg-white border-0">
                </div>
                <div class="col-md-2 d-flex align-items-end gap-2">
                  <button @click="actualizarSintoma" class="btn btn-success w-100" :disabled="guardandoSintoma">
                    <span v-if="guardandoSintoma" class="spinner-border spinner-border-sm"></span>
                    <span v-else>Guardar</span>
                  </button>
                  <button @click="cancelarEditSintoma" class="btn btn-light border w-100">Cancelar</button>
                </div>
              </div>
              <div class="mt-3">
                <label class="form-label text-muted small fw-bold text-uppercase">Notas</label>
                <textarea v-model="editSintomaForm.notes" class="form-control bg-white border-0" rows="2"></textarea>
              </div>
            </div>

            <ConfirmDialog
              :visible="deleteSintomaDialog"
              title="Eliminar Síntoma"
              message="¿Está seguro que desea eliminar este síntoma? Esta acción no se puede deshacer."
              confirmText="Eliminar"
              :danger="true"
              @confirm="eliminarSintoma"
              @cancel="deleteSintomaDialog = false"
            />
          </div>

          <!-- SIGNOS VITALES TAB -->
          <div v-if="tabActual === 'signos'" class="animation-fade">
            <div class="d-flex justify-content-between align-items-center mb-4">
              <div>
                <h5 class="fw-bold text-dark mb-0">Mediciones Clínicas</h5>
                <p class="text-muted small mb-0" v-if="signos.length">Última toma: {{ formatDateTime(signos[signos.length - 1]?.measuredAt) }}</p>
              </div>
              <button @click="toggleFormSignos" class="btn btn-primary btn-sm px-3 shadow-sm">
                {{ showSignosForm ? 'Cancelar' : '+ Registrar Medición' }}
              </button>
            </div>

            <div v-if="showSignosForm" class="card bg-light border-0 p-4 rounded-4 mb-4">
              <div class="row g-3">
                <div class="col-md-4">
                  <label class="form-label text-muted small fw-bold text-uppercase">Temperatura (°C)</label>
                  <input v-model.number="signosForm.temperature" type="number" step="0.1" class="form-control bg-white border-0" placeholder="36.5">
                </div>
                <div class="col-md-4">
                  <label class="form-label text-muted small fw-bold text-uppercase">Frec. Cardíaca (lpm)</label>
                  <input v-model.number="signosForm.heartRate" type="number" class="form-control bg-white border-0" placeholder="72">
                </div>
                <div class="col-md-4">
                  <label class="form-label text-muted small fw-bold text-uppercase">Presión Sistólica</label>
                  <input v-model.number="signosForm.systolicPressure" type="number" class="form-control bg-white border-0" placeholder="120">
                </div>
                <div class="col-md-4">
                  <label class="form-label text-muted small fw-bold text-uppercase">Presión Diastólica</label>
                  <input v-model.number="signosForm.diastolicPressure" type="number" class="form-control bg-white border-0" placeholder="80">
                </div>
                <div class="col-md-4">
                  <label class="form-label text-muted small fw-bold text-uppercase">Saturación O2 (%)</label>
                  <input v-model.number="signosForm.oxygenSaturation" type="number" step="0.1" class="form-control bg-white border-0" placeholder="98">
                </div>
                <div class="col-md-4">
                  <label class="form-label text-muted small fw-bold text-uppercase">Glucosa (mg/dL)</label>
                  <input v-model.number="signosForm.glucose" type="number" class="form-control bg-white border-0" placeholder="95">
                </div>
              </div>
              <div class="d-flex justify-content-end mt-4">
                <button @click="guardarSignos" class="btn btn-success px-4 shadow-sm" :disabled="guardandoSignos">
                  <span v-if="guardandoSignos" class="spinner-border spinner-border-sm me-2"></span>
                  Guardar Medición
                </button>
              </div>
            </div>

            <div v-if="signos.length === 0 && !showSignosForm" class="text-muted text-center py-4">
              <p>No hay signos vitales registrados para esta cita.</p>
            </div>

            <div v-if="signos.length" class="row g-4 text-center mt-2">
              <div class="col-md-4 col-sm-6">
                <div class="card bg-light border-0 shadow-sm p-4 rounded-4 h-100">
                  <div class="display-6 mb-2">🌡️</div>
                  <h6 class="text-muted fw-bold text-uppercase mb-1" style="font-size: 0.8rem;">Temperatura</h6>
                  <h3 class="fw-bold mb-0" :class="ultimoSigno.temperature >= 37.5 ? 'text-danger' : 'text-dark'">{{ ultimoSigno.temperature }} °C</h3>
                </div>
              </div>
              <div class="col-md-4 col-sm-6">
                <div class="card bg-light border-0 shadow-sm p-4 rounded-4 h-100">
                  <div class="display-6 mb-2">💓</div>
                  <h6 class="text-muted fw-bold text-uppercase mb-1" style="font-size: 0.8rem;">Frec. Cardíaca</h6>
                  <h3 class="fw-bold text-dark mb-0">{{ ultimoSigno.heartRate }} lpm</h3>
                </div>
              </div>
              <div class="col-md-4 col-sm-6">
                <div class="card bg-light border-0 shadow-sm p-4 rounded-4 h-100">
                  <div class="display-6 mb-2">🩸</div>
                  <h6 class="text-muted fw-bold text-uppercase mb-1" style="font-size: 0.8rem;">Presión Arterial</h6>
                  <h3 class="fw-bold text-dark mb-0">{{ ultimoSigno.systolicPressure }}/{{ ultimoSigno.diastolicPressure }}</h3>
                  <small class="text-muted fw-medium">mmHg</small>
                </div>
              </div>
              <div class="col-md-6 col-sm-6">
                <div class="card bg-light border-0 shadow-sm p-4 rounded-4 h-100">
                  <div class="display-6 mb-2">🫁</div>
                  <h6 class="text-muted fw-bold text-uppercase mb-1" style="font-size: 0.8rem;">Saturación O2</h6>
                  <h3 class="fw-bold mb-0" :class="ultimoSigno.oxygenSaturation < 95 ? 'text-warning' : 'text-dark'">{{ ultimoSigno.oxygenSaturation }} %</h3>
                </div>
              </div>
              <div class="col-md-6 col-sm-6">
                <div class="card bg-light border-0 shadow-sm p-4 rounded-4 h-100">
                  <div class="display-6 mb-2">🍬</div>
                  <h6 class="text-muted fw-bold text-uppercase mb-1" style="font-size: 0.8rem;">Glucosa</h6>
                  <h3 class="fw-bold text-dark mb-0">{{ ultimoSigno.glucose || 'N/A' }}</h3>
                  <small class="text-muted fw-medium">mg/dL</small>
                </div>
              </div>
            </div>

            <div v-if="signos.length > 1" class="mt-4">
              <h6 class="fw-bold text-muted mb-3">Mediciones Anteriores</h6>
              <div class="table-responsive">
                <table class="table table-sm table-hover align-middle mb-0">
                  <thead class="bg-light text-muted">
                    <tr>
                      <th class="py-2">Fecha</th>
                      <th class="py-2">Temp</th>
                      <th class="py-2">FC</th>
                      <th class="py-2">PA</th>
                      <th class="py-2">O2</th>
                      <th class="py-2">Glucosa</th>
                      <th class="py-2">Acción</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="s in signos.slice().reverse()" :key="s.id">
                      <td class="py-2 text-muted small">{{ formatDateTime(s.measuredAt) }}</td>
                      <td class="py-2">{{ s.temperature }}°C</td>
                      <td class="py-2">{{ s.heartRate }} lpm</td>
                      <td class="py-2">{{ s.systolicPressure }}/{{ s.diastolicPressure }}</td>
                      <td class="py-2">{{ s.oxygenSaturation }}%</td>
                      <td class="py-2">{{ s.glucose || '-' }}</td>
                      <td class="py-2">
                        <button @click="editarSigno(s)" class="btn btn-sm btn-light border text-warning px-2">Editar</button>
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>

            <div v-if="editSignoId" class="card bg-light border-0 p-4 rounded-4 mt-4">
              <h6 class="fw-bold mb-3">Editar Medición</h6>
              <div class="row g-3">
                <div class="col-md-4">
                  <label class="form-label text-muted small fw-bold text-uppercase">Temperatura (°C)</label>
                  <input v-model.number="editSignoForm.temperature" type="number" step="0.1" class="form-control bg-white border-0">
                </div>
                <div class="col-md-4">
                  <label class="form-label text-muted small fw-bold text-uppercase">Frec. Cardíaca (lpm)</label>
                  <input v-model.number="editSignoForm.heartRate" type="number" class="form-control bg-white border-0">
                </div>
                <div class="col-md-4">
                  <label class="form-label text-muted small fw-bold text-uppercase">Presión Sistólica</label>
                  <input v-model.number="editSignoForm.systolicPressure" type="number" class="form-control bg-white border-0">
                </div>
                <div class="col-md-4">
                  <label class="form-label text-muted small fw-bold text-uppercase">Presión Diastólica</label>
                  <input v-model.number="editSignoForm.diastolicPressure" type="number" class="form-control bg-white border-0">
                </div>
                <div class="col-md-4">
                  <label class="form-label text-muted small fw-bold text-uppercase">Saturación O2 (%)</label>
                  <input v-model.number="editSignoForm.oxygenSaturation" type="number" step="0.1" class="form-control bg-white border-0">
                </div>
                <div class="col-md-4">
                  <label class="form-label text-muted small fw-bold text-uppercase">Glucosa (mg/dL)</label>
                  <input v-model.number="editSignoForm.glucose" type="number" class="form-control bg-white border-0">
                </div>
              </div>
              <div class="d-flex justify-content-end gap-2 mt-4">
                <button @click="cancelarEditSigno" class="btn btn-light border px-4">Cancelar</button>
                <button @click="actualizarSigno" class="btn btn-success px-4 shadow-sm" :disabled="guardandoSignos">
                  <span v-if="guardandoSignos" class="spinner-border spinner-border-sm me-2"></span>
                  Guardar Cambios
                </button>
              </div>
            </div>
          </div>

          <!-- HISTORIAL MÉDICO TAB -->
          <div v-if="tabActual === 'records'" class="animation-fade">
            <div class="d-flex justify-content-between align-items-center mb-4">
              <h5 class="fw-bold text-dark mb-0">Registros Médicos</h5>
              <button @click="toggleFormRecord" class="btn btn-primary btn-sm px-3 shadow-sm">
                {{ showRecordForm ? 'Cancelar' : '+ Nuevo Registro' }}
              </button>
            </div>

            <div v-if="showRecordForm" class="card bg-light border-0 p-4 rounded-4 mb-4">
              <h6 class="fw-bold mb-3">{{ editRecordId ? 'Editar' : 'Nuevo' }} Registro Médico</h6>
              <div class="row g-3">
                <div class="col-md-6">
                  <label class="form-label text-muted small fw-bold text-uppercase">Diagnóstico Inicial</label>
                  <input v-model="recordForm.diagnosisInitial" type="text" class="form-control bg-white border-0" placeholder="Diagnóstico principal">
                </div>
                <div class="col-md-6">
                  <label class="form-label text-muted small fw-bold text-uppercase">Antecedentes</label>
                  <input v-model="recordForm.antecedentes" type="text" class="form-control bg-white border-0" placeholder="Antecedentes del paciente">
                </div>
                <div class="col-md-12">
                  <label class="form-label text-muted small fw-bold text-uppercase">Notas</label>
                  <textarea v-model="recordForm.notes" class="form-control bg-white border-0" rows="3" placeholder="Notas del registro médico"></textarea>
                </div>
                <div class="col-md-12">
                  <label class="form-label text-muted small fw-bold text-uppercase">Observaciones de Consulta</label>
                  <textarea v-model="recordForm.observacionesConsulta" class="form-control bg-white border-0" rows="2" placeholder="Observaciones adicionales"></textarea>
                </div>
              </div>
              <div class="d-flex justify-content-end gap-2 mt-4">
                <button type="button" @click="cancelarFormRecord" class="btn btn-light border px-4">Cancelar</button>
                <button @click="guardarRecord" class="btn btn-success px-4 shadow-sm" :disabled="guardandoRecord">
                  <span v-if="guardandoRecord" class="spinner-border spinner-border-sm me-2"></span>
                  {{ editRecordId ? 'Actualizar' : 'Guardar' }}
                </button>
              </div>
            </div>

            <div v-if="recordsMedicos.length === 0" class="text-muted text-center py-4">
              <p>No hay registros médicos para esta cita.</p>
            </div>
            <div v-for="r in recordsMedicos" :key="r.id" class="card bg-light border-0 p-4 rounded-4 mb-3">
              <div class="d-flex justify-content-between mb-2">
                <small class="text-muted fw-bold">{{ formatDate(r.createdAt) }}</small>
                <div class="d-flex gap-2">
                  <button @click="editarRecord(r)" class="btn btn-sm btn-light border text-warning px-3">Editar</button>
                </div>
              </div>
              <p class="mb-1"><strong>Diagnóstico Inicial:</strong> {{ r.diagnosisInitial || 'N/A' }}</p>
              <p class="mb-1"><strong>Notas:</strong> {{ r.notes || 'N/A' }}</p>
              <p v-if="r.antecedentes" class="mb-1"><strong>Antecedentes:</strong> {{ r.antecedentes }}</p>
              <p v-if="r.observacionesConsulta" class="mb-0"><strong>Observaciones:</strong> {{ r.observacionesConsulta }}</p>
            </div>
          </div>

          <!-- ALERTAS TAB -->
          <div v-if="tabActual === 'alertas'" class="animation-fade">
            <div class="d-flex justify-content-between align-items-center mb-4">
              <h5 class="fw-bold text-dark mb-0">Alertas de la Cita</h5>
            </div>
            <div v-if="cargandoAlertas" class="text-center py-4">
              <div class="spinner-border text-primary" role="status"></div>
            </div>
            <div v-else-if="alertas.length === 0" class="text-muted text-center py-4">
              <p>No hay alertas asociadas a esta cita.</p>
            </div>
            <div v-for="a in alertas" :key="a.id" class="card mb-3 border-0 shadow-sm rounded-4" :class="a.isResolved ? 'bg-light' : 'border-start border-4 border-danger'">
              <div class="card-body p-4">
                <div class="d-flex justify-content-between mb-2">
                  <h6 class="fw-bold mb-0">{{ a.alertType || 'Alerta' }}</h6>
                  <span class="badge rounded-pill px-3" :class="a.isResolved ? 'bg-success' : 'bg-danger'">
                    {{ a.isResolved ? 'Resuelta' : 'Activa' }}
                  </span>
                </div>
                <p class="text-muted mb-2" v-if="a.description">{{ a.description }}</p>
                <small class="text-muted">{{ formatDate(a.createdAt) }}</small>
              </div>
            </div>
          </div>

          <!-- RECOMENDACIONES TAB -->
          <div v-if="tabActual === 'recomendaciones'" class="animation-fade">
            <div class="d-flex justify-content-between align-items-center mb-4">
              <h5 class="fw-bold text-dark mb-0">Recomendaciones</h5>
            </div>
            <div v-if="cargandoRecomendaciones" class="text-center py-4">
              <div class="spinner-border text-primary" role="status"></div>
            </div>
            <div v-else-if="recomendaciones.length === 0" class="text-muted text-center py-4">
              <p>No hay recomendaciones para esta cita.</p>
            </div>
            <div v-for="r in recomendaciones" :key="r.id" class="card bg-light border-0 border-start border-4 border-info p-4 rounded-4 mb-3">
              <h6 class="fw-bold mb-2">{{ r.title || 'Recomendación' }}</h6>
              <p class="text-muted mb-2" v-if="r.description">{{ r.description }}</p>
              <small class="text-muted">{{ formatDate(r.createdAt) }}</small>
            </div>
          </div>

          <!-- DOCUMENTOS TAB -->
          <div v-if="tabActual === 'documentos'" class="animation-fade">
            <div class="d-flex justify-content-between align-items-center mb-4">
              <h5 class="fw-bold text-dark mb-0">Documentos</h5>
              <button @click="showDocUpload = !showDocUpload" class="btn btn-primary btn-sm px-3 shadow-sm">
                {{ showDocUpload ? 'Cancelar' : '+ Subir Documento' }}
              </button>
            </div>

            <div v-if="showDocUpload" class="card bg-light border-0 p-4 rounded-4 mb-4">
              <form @submit.prevent="subirDocumento">
                <div class="row g-3">
                  <div class="col-md-8">
                    <label class="form-label text-muted small fw-bold text-uppercase">Archivo</label>
                    <input ref="docFileInput" type="file" class="form-control bg-white border-0" accept=".pdf,.jpg,.jpeg,.png" required>
                    <small class="text-muted">PDF, JPG o PNG. Máximo 10MB.</small>
                  </div>
                  <div class="col-md-4 d-flex align-items-end">
                    <button type="submit" class="btn btn-success w-100 shadow-sm" :disabled="subiendoDoc">
                      <span v-if="subiendoDoc" class="spinner-border spinner-border-sm me-2"></span>
                      Subir
                    </button>
                  </div>
                </div>
              </form>
            </div>

            <div v-if="cargandoDocs" class="text-center py-4">
              <div class="spinner-border text-primary" role="status"></div>
            </div>
            <div v-else-if="documentos.length === 0" class="text-muted text-center py-4">
              <p>No hay documentos asociados a esta cita.</p>
            </div>
            <div v-else class="row g-4">
              <div class="col-md-4 col-sm-6" v-for="d in documentos" :key="d.id">
                <div class="card border-0 shadow-sm rounded-4 h-100">
                  <div class="card-body p-4 text-center">
                    <div class="display-5 mb-2">{{ fileIcon(d.fileName) }}</div>
                    <h6 class="fw-bold small mb-1">{{ d.fileName }}</h6>
                    <small class="text-muted d-block mb-2">{{ formatDate(d.uploadedAt) }}</small>
                    <div class="d-flex justify-content-center gap-2">
                      <a :href="d.fileUrl" target="_blank" class="btn btn-sm btn-light border px-3">Ver</a>
                      <button @click="confirmarEliminarDoc(d)" class="btn btn-sm btn-light border text-danger px-3">Eliminar</button>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- IA TAB -->
          <div v-if="tabActual === 'ia'" class="animation-fade">
            <div v-if="analisisIA.length === 0 && !generandoIA" class="text-center py-5">
              <div class="display-4 mb-3">🧠</div>
              <h4 class="fw-bold">Asistente de Diagnóstico</h4>
              <p class="text-muted w-50 mx-auto">La Inteligencia Artificial analizará el cuadro clínico, signos vitales y síntomas para sugerir recomendaciones y calcular factores de riesgo.</p>
              <button @click="solicitarAnalisisIA" class="btn btn-dark btn-lg px-5 mt-3 shadow">
                <span v-if="solicitandoIA" class="spinner-border spinner-border-sm me-2"></span>
                <span v-if="solicitandoIA">Procesando modelo...</span>
                <span v-else>Generar Reporte IA</span>
              </button>
            </div>

            <div v-if="generandoIA" class="text-center py-5">
              <div class="spinner-border text-dark mb-3" style="width: 3rem; height: 3rem;" role="status"></div>
              <p class="text-muted">Generando análisis con inteligencia artificial...</p>
            </div>

            <div v-for="(ia, idx) in analisisIA" :key="ia.id || idx">
              <div class="bg-light p-4 rounded-4 border-start border-5 position-relative mb-4" :class="ia.isReviewed ? 'border-success' : 'border-info'">
                <span v-if="ia.isReviewed" class="badge bg-success position-absolute top-0 end-0 m-3 px-3 py-2 rounded-pill">✅ Revisado</span>
                <span v-else class="badge bg-info position-absolute top-0 end-0 m-3 px-3 py-2 rounded-pill">🆕 Nuevo</span>

                <h5 class="fw-bold mb-3">🤖 {{ ia.analysisType || 'Análisis Clínico' }}</h5>

                <div class="mb-3">
                  <h6 class="fw-bold text-dark mb-1">Modelo utilizado:</h6>
                  <p class="text-muted">{{ ia.modelUsed || 'Modelo por defecto' }}</p>
                </div>

                <div class="mb-3" v-if="ia.aiResponse">
                  <h6 class="fw-bold text-dark mb-1">Resultado:</h6>
                  <p class="text-muted" style="white-space: pre-wrap;">{{ ia.aiResponse }}</p>
                </div>

                <div class="mb-3" v-if="ia.promptUsed">
                  <h6 class="fw-bold text-dark mb-1">Prompt utilizado:</h6>
                  <p class="text-muted small">{{ ia.promptUsed }}</p>
                </div>

                <div v-if="!ia.isReviewed" class="d-flex justify-content-end mt-3">
                  <button @click="marcarRevisado(ia)" class="btn btn-success btn-sm px-4 rounded-pill">Marcar como Revisado</button>
                </div>
              </div>
            </div>
          </div>

        </div>
      </div>
    </template>
    <ConfirmDialog
      :visible="deleteDocDialog"
      title="Eliminar Documento"
      :message="`¿Está seguro que desea eliminar el documento ${deleteDocTarget?.fileName}?`"
      confirmText="Eliminar"
      :danger="true"
      @confirm="eliminarDoc"
      @cancel="deleteDocDialog = false"
    />
  </div>
</template>

<script setup>
import { ref, reactive, computed, watch, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { appointmentService } from '@/api/appointments'
import { medicalRecordService } from '@/api/medicalRecords'
import { symptomService } from '@/api/symptoms'
import { vitalSignService } from '@/api/vitalSigns'
import { aiAnalysisService } from '@/api/aiAnalysis'
import { patientService } from '@/api/patients'
import { alertService } from '@/api/alerts'
import { recommendationService } from '@/api/recommendations'
import { medicalDocumentService } from '@/api/medicalDocuments'
import StatusBadge from '@/components/StatusBadge.vue'
import ConfirmDialog from '@/components/ConfirmDialog.vue'

const props = defineProps({
  id: { type: String, required: true }
})

const router = useRouter()

const loading = ref(true)
const error = ref('')
const cita = ref(null)
const patientName = ref('')
const tabActual = ref('info')
const nuevoEstado = ref('Pending')

const guardandoNotas = ref(false)
const notasGuardadas = ref(false)
const notasEdit = ref('')

const sinstomasCargados = ref(false)
const showSintomaForm = ref(false)
const guardandoSintoma = ref(false)
const sintomas = ref([])
const sintomaForm = reactive({
  name: '', severity: 'Moderado', startedAt: '', notes: ''
})

const showSignosForm = ref(false)
const guardandoSignos = ref(false)
const signos = ref([])
const signosForm = reactive({
  temperature: 36.5, heartRate: 72, systolicPressure: 120,
  diastolicPressure: 80, oxygenSaturation: 98, glucose: null
})

const analisisIA = ref([])
const solicitandoIA = ref(false)
const generandoIA = ref(false)

const editSintomaId = ref(null)
const editSintomaForm = reactive({ name: '', severity: 'Moderado', startedAt: '', notes: '' })
const deleteSintomaDialog = ref(false)
const deleteSintomaTarget = ref(null)
const deleteSintomaLoading = ref(false)

const editSignoId = ref(null)
const editSignoForm = reactive({ temperature: 36.5, heartRate: 72, systolicPressure: 120, diastolicPressure: 80, oxygenSaturation: 98, glucose: null })

const recordsMedicos = ref([])
const showRecordForm = ref(false)
const editRecordId = ref(null)
const guardandoRecord = ref(false)
const recordForm = reactive({ diagnosisInitial: '', notes: '', antecedentes: '', observacionesConsulta: '' })

const alertas = ref([])
const cargandoAlertas = ref(false)

const recomendaciones = ref([])
const cargandoRecomendaciones = ref(false)

const documentos = ref([])
const cargandoDocs = ref(false)
const showDocUpload = ref(false)
const subiendoDoc = ref(false)
const docFileInput = ref(null)
const deleteDocDialog = ref(false)
const deleteDocTarget = ref(null)

const statusVariant = (status) => {
  const map = { Pending: 'pending', InProgress: 'inprogress', Completed: 'completed', Cancelled: 'cancelled' }
  return map[status] || 'secondary'
}

const ultimoSigno = computed(() => {
  if (!signos.value.length) return { temperature: 0, heartRate: 0, systolicPressure: 0, diastolicPressure: 0, oxygenSaturation: 0, glucose: null }
  return signos.value[signos.value.length - 1]
})

const formatDateTime = (dateStr) => {
  if (!dateStr) return '-'
  try {
    return new Date(dateStr).toLocaleDateString('es-ES', {
      day: '2-digit', month: 'short', year: 'numeric', hour: '2-digit', minute: '2-digit'
    })
  } catch { return dateStr }
}

const formatDate = (dateStr) => {
  if (!dateStr) return '-'
  try {
    return new Date(dateStr).toLocaleDateString('es-ES', { day: '2-digit', month: 'short', year: 'numeric' })
  } catch { return dateStr }
}

const severityColor = (sev) => {
  const map = { Leve: 'success', Moderado: 'warning', Severo: 'danger' }
  return map[sev] || 'secondary'
}

const severityBadge = (sev) => {
  const map = { Leve: 'bg-success', Moderado: 'bg-warning text-dark', Severo: 'bg-danger' }
  return map[sev] || 'bg-secondary'
}

const cargarDetalle = async () => {
  loading.value = true
  error.value = ''
  try {
    const res = await appointmentService.getDetail(props.id)
    cita.value = res.data
    nuevoEstado.value = res.data.status || 'Pending'
    notasEdit.value = res.data.notes || ''

    if (res.data.patientId) {
      try {
        const pRes = await patientService.getById(res.data.patientId)
        patientName.value = pRes.data?.fullName || `Paciente #${res.data.patientId}`
      } catch {
        patientName.value = `Paciente #${res.data.patientId}`
      }
    }

    sintomas.value = res.data.symptoms || []
    sinstomasCargados.value = true
    signos.value = res.data.vitalSigns || []
    analisisIA.value = res.data.aiAnalyses || []
  } catch (err) {
    if (err.response?.status === 404) {
      error.value = 'Cita no encontrada.'
    } else {
      error.value = 'Error al cargar detalle de la cita.'
    }
  } finally {
    loading.value = false
  }
}

const guardarNotas = async () => {
  guardandoNotas.value = true
  notasGuardadas.value = false
  try {
    await appointmentService.update(Number(props.id), {
      patientId: cita.value.patientId,
      doctorId: cita.value.doctorId,
      appointmentDate: cita.value.appointmentDate,
      status: cita.value.status,
      reason: cita.value.reason,
      notes: notasEdit.value
    })
    notasGuardadas.value = true
    cita.value.notes = notasEdit.value
    setTimeout(() => { notasGuardadas.value = false }, 3000)
  } catch (err) {
    error.value = 'Error al guardar las notas'
  } finally {
    guardandoNotas.value = false
  }
}

const cambiarEstado = async () => {
  try {
    await appointmentService.changeStatus(Number(props.id), nuevoEstado.value)
    cita.value.status = nuevoEstado.value
  } catch (err) {
    nuevoEstado.value = cita.value.status
    error.value = 'Error al cambiar estado'
  }
}

const toggleFormSintoma = () => {
  showSintomaForm.value = !showSintomaForm.value
  if (!showSintomaForm.value) {
    sintomaForm.name = ''
    sintomaForm.severity = 'Moderado'
    sintomaForm.startedAt = ''
    sintomaForm.notes = ''
  }
}

const guardarSintoma = async () => {
  if (!sintomaForm.name || !sintomaForm.startedAt) return
  guardandoSintoma.value = true
  try {
    const res = await symptomService.create({
      appointmentId: Number(props.id),
      name: sintomaForm.name,
      severity: sintomaForm.severity,
      startedAt: sintomaForm.startedAt,
      notes: sintomaForm.notes || null
    })
    sintomas.value.push(res.data)
    sintomaForm.name = ''
    sintomaForm.startedAt = ''
    sintomaForm.notes = ''
    showSintomaForm.value = false
  } catch (err) {
    error.value = 'Error al guardar síntoma'
  } finally {
    guardandoSintoma.value = false
  }
}

const toggleFormSignos = () => {
  showSignosForm.value = !showSignosForm.value
  if (!showSignosForm.value) {
    signosForm.temperature = 36.5
    signosForm.heartRate = 72
    signosForm.systolicPressure = 120
    signosForm.diastolicPressure = 80
    signosForm.oxygenSaturation = 98
    signosForm.glucose = null
  }
}

const guardarSignos = async () => {
  guardandoSignos.value = true
  try {
    const res = await vitalSignService.create({
      appointmentId: Number(props.id),
      temperature: signosForm.temperature,
      heartRate: signosForm.heartRate,
      systolicPressure: signosForm.systolicPressure,
      diastolicPressure: signosForm.diastolicPressure,
      oxygenSaturation: signosForm.oxygenSaturation,
      glucose: signosForm.glucose || null
    })
    signos.value.push(res.data)
    showSignosForm.value = false
  } catch (err) {
    error.value = 'Error al guardar signos vitales'
  } finally {
    guardandoSignos.value = false
  }
}

const solicitarAnalisisIA = async () => {
  solicitandoIA.value = true
  try {
    await aiAnalysisService.getByAppointment(props.id)
    generandoIA.value = true
    setTimeout(async () => {
      try {
        const res = await appointmentService.getDetail(props.id)
        analisisIA.value = res.data.aiAnalyses || []
      } catch {
        analisisIA.value = []
      }
      generandoIA.value = false
      solicitandoIA.value = false
    }, 2000)
  } catch {
    solicitandoIA.value = false
    error.value = 'Error al solicitar análisis IA'
  }
}

const marcarRevisado = async (ia) => {
  try {
    await aiAnalysisService.markAsReviewed(ia.id)
    ia.isReviewed = true
  } catch (err) {
    error.value = 'Error al marcar como revisado'
  }
}

const editarSintoma = (s) => {
  editSintomaId.value = s.id
  editSintomaForm.name = s.name
  editSintomaForm.severity = s.severity
  editSintomaForm.startedAt = s.startedAt ? s.startedAt.substring(0, 10) : ''
  editSintomaForm.notes = s.notes || ''
}

const cancelarEditSintoma = () => {
  editSintomaId.value = null
}

const actualizarSintoma = async () => {
  guardandoSintoma.value = true
  try {
    const res = await symptomService.update(editSintomaId.value, { ...editSintomaForm })
    const idx = sintomas.value.findIndex(s => s.id === editSintomaId.value)
    if (idx !== -1) sintomas.value[idx] = res.data
    editSintomaId.value = null
  } catch (err) {
    error.value = 'Error al actualizar síntoma'
  } finally {
    guardandoSintoma.value = false
  }
}

const confirmarEliminarSintoma = (s) => {
  deleteSintomaTarget.value = s
  deleteSintomaDialog.value = true
}

const eliminarSintoma = async () => {
  deleteSintomaLoading.value = true
  try {
    await symptomService.remove(deleteSintomaTarget.value.id)
    sintomas.value = sintomas.value.filter(s => s.id !== deleteSintomaTarget.value.id)
  } catch (err) {
    error.value = 'Error al eliminar síntoma'
  } finally {
    deleteSintomaDialog.value = false
    deleteSintomaLoading.value = false
  }
}

const editarSigno = (s) => {
  editSignoId.value = s.id
  editSignoForm.temperature = s.temperature
  editSignoForm.heartRate = s.heartRate
  editSignoForm.systolicPressure = s.systolicPressure
  editSignoForm.diastolicPressure = s.diastolicPressure
  editSignoForm.oxygenSaturation = s.oxygenSaturation
  editSignoForm.glucose = s.glucose
}

const cancelarEditSigno = () => {
  editSignoId.value = null
}

const actualizarSigno = async () => {
  guardandoSignos.value = true
  try {
    const res = await vitalSignService.update(editSignoId.value, { ...editSignoForm })
    const idx = signos.value.findIndex(s => s.id === editSignoId.value)
    if (idx !== -1) signos.value[idx] = res.data
    editSignoId.value = null
  } catch (err) {
    error.value = 'Error al actualizar signos vitales'
  } finally {
    guardandoSignos.value = false
  }
}

const toggleFormRecord = () => {
  showRecordForm.value = !showRecordForm.value
  if (!showRecordForm.value) cancelarFormRecord()
}

const cancelarFormRecord = () => {
  showRecordForm.value = false
  editRecordId.value = null
  recordForm.diagnosisInitial = ''
  recordForm.notes = ''
  recordForm.antecedentes = ''
  recordForm.observacionesConsulta = ''
}

const guardarRecord = async () => {
  guardandoRecord.value = true
  try {
    const payload = {
      appointmentId: Number(props.id),
      patientId: cita.value.patientId,
      diagnosisInitial: recordForm.diagnosisInitial || null,
      notes: recordForm.notes || null,
      antecedentes: recordForm.antecedentes || null,
      observacionesConsulta: recordForm.observacionesConsulta || null
    }
    if (editRecordId.value) {
      const res = await medicalRecordService.update(editRecordId.value, payload)
      const idx = recordsMedicos.value.findIndex(r => r.id === editRecordId.value)
      if (idx !== -1) recordsMedicos.value[idx] = res.data
    } else {
      const res = await medicalRecordService.create(payload)
      recordsMedicos.value.push(res.data)
    }
    cancelarFormRecord()
  } catch (err) {
    error.value = 'Error al guardar registro médico'
  } finally {
    guardandoRecord.value = false
  }
}

const editarRecord = (r) => {
  editRecordId.value = r.id
  recordForm.diagnosisInitial = r.diagnosisInitial || ''
  recordForm.notes = r.notes || ''
  recordForm.antecedentes = r.antecedentes || ''
  recordForm.observacionesConsulta = r.observacionesConsulta || ''
  showRecordForm.value = true
}

const fileIcon = (name) => {
  if (!name) return '📄'
  const ext = name.split('.').pop()?.toLowerCase()
  if (ext === 'pdf') return '📕'
  if (['jpg', 'jpeg', 'png'].includes(ext)) return '🖼️'
  return '📄'
}

const confirmarEliminarDoc = (d) => {
  deleteDocTarget.value = d
  deleteDocDialog.value = true
}

const subirDocumento = async () => {
  const file = docFileInput.value?.files?.[0]
  if (!file) return
  subiendoDoc.value = true
  try {
    const formData = new FormData()
    formData.append('file', file)
    formData.append('appointmentId', props.id)
    formData.append('patientId', cita.value.patientId)
    await medicalDocumentService.upload(formData)
    showDocUpload.value = false
    docFileInput.value.value = ''
    await cargarDocumentos()
  } catch (err) {
    error.value = 'Error al subir el documento'
  } finally {
    subiendoDoc.value = false
  }
}

const cargarAlertas = async () => {
  cargandoAlertas.value = true
  try {
    const res = await alertService.getByAppointment(props.id)
    alertas.value = res.data || []
  } catch {
    alertas.value = []
  } finally {
    cargandoAlertas.value = false
  }
}

const cargarRecomendaciones = async () => {
  cargandoRecomendaciones.value = true
  try {
    const res = await recommendationService.getByAppointment(props.id)
    recomendaciones.value = res.data || []
  } catch {
    recomendaciones.value = []
  } finally {
    cargandoRecomendaciones.value = false
  }
}

const cargarDocumentos = async () => {
  cargandoDocs.value = true
  try {
    const res = await medicalDocumentService.getByAppointment(props.id)
    documentos.value = res.data || []
  } catch {
    documentos.value = []
  } finally {
    cargandoDocs.value = false
  }
}

const eliminarDoc = async () => {
  try {
    await medicalDocumentService.remove(deleteDocTarget.value.id)
    documentos.value = documentos.value.filter(d => d.id !== deleteDocTarget.value.id)
  } catch (err) {
    error.value = 'Error al eliminar el documento'
  } finally {
    deleteDocDialog.value = false
  }
}

watch(tabActual, (tab) => {
  if (tab === 'alertas' && alertas.value.length === 0) cargarAlertas()
  if (tab === 'recomendaciones' && recomendaciones.value.length === 0) cargarRecomendaciones()
  if (tab === 'documentos' && documentos.value.length === 0) cargarDocumentos()
  if (tab === 'records' && recordsMedicos.value.length === 0 && cita.value?.medicalRecords) {
    recordsMedicos.value = cita.value.medicalRecords || []
  }
})

onMounted(() => {
  cargarDetalle()
})
</script>

<style scoped>
.animation-fade { animation: fadeIn 0.3s ease-in-out; }
@keyframes fadeIn { from { opacity: 0; transform: translateY(5px); } to { opacity: 1; transform: translateY(0); } }
input:focus, select:focus, textarea:focus { background-color: #fff !important; box-shadow: 0 0 0 0.25rem rgba(14, 165, 233, 0.25); }
</style>
