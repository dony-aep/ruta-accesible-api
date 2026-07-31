using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Data
{
    /// <summary>
    /// Datos iniciales de la API: el catálogo cerrado de criterios de la NTC 6047 y
    /// lugares reales de Barranquilla con reportes ciudadanos en texto libre.
    /// La mayoría de los reportes queda en estado Registrado a propósito: son el
    /// insumo del endpoint de análisis con IA (PLAN-02).
    /// </summary>
    public static class SeedData
    {
        public static async Task InicializarAsync(AppDbContext contexto)
        {
            await SembrarTiposBarreraAsync(contexto);
            await SembrarLugaresAsync(contexto);
        }

        /// <summary>
        /// Los diez criterios de la NTC 6047:2013. Es la lista cerrada contra la que
        /// clasifica el modelo de lenguaje; ver docs/referencia-ntc6047.md, sección 2.
        /// Las categorías son fiables; las medidas exactas de la norma no están
        /// verificadas, por eso no aparecen aquí.
        /// </summary>
        private static async Task SembrarTiposBarreraAsync(AppDbContext contexto)
        {
            if (await contexto.TiposBarrera.AnyAsync()) return;

            var tipos = new List<TipoBarrera>
            {
                new TipoBarrera
                {
                    Codigo = "NTC-ESTACIONAMIENTOS",
                    Nombre = "Estacionamientos accesibles",
                    CriterioNorma = "Plazas reservadas cerca de la entrada, con dimensiones que permitan la transferencia desde una silla de ruedas y con ruta accesible hasta el acceso."
                },
                new TipoBarrera
                {
                    Codigo = "NTC-CIRCULACION",
                    Nombre = "Circulación horizontal",
                    CriterioNorma = "Pasillos y senderos amplios, a nivel y libres de obstáculos, que permitan el desplazamiento autónomo."
                },
                new TipoBarrera
                {
                    Codigo = "NTC-RAMPAS",
                    Nombre = "Rampas",
                    CriterioNorma = "Pendiente y ancho reglamentarios, superficie antideslizante y pasamanos, como alternativa accesible a los desniveles."
                },
                new TipoBarrera
                {
                    Codigo = "NTC-ESCALERAS",
                    Nombre = "Escaleras",
                    CriterioNorma = "Huella y contrahuella limitadas, pasamanos en ambos lados y señalización del inicio y el fin del tramo."
                },
                new TipoBarrera
                {
                    Codigo = "NTC-ASCENSORES",
                    Nombre = "Ascensores y plataformas elevadoras",
                    CriterioNorma = "Espacio interior suficiente para maniobrar y controles alcanzables desde una silla de ruedas, con información sonora y visual."
                },
                new TipoBarrera
                {
                    Codigo = "NTC-PUERTAS",
                    Nombre = "Pasillos y puertas",
                    CriterioNorma = "Ancho libre mínimo de paso, mecanismos de apertura operables sin fuerza excesiva y ausencia de obstáculos en el recorrido."
                },
                new TipoBarrera
                {
                    Codigo = "NTC-BANOS",
                    Nombre = "Cuartos de baño accesibles",
                    CriterioNorma = "Área de maniobra y barras de apoyo en los tipos A, B y C definidos por la norma, con accesorios al alcance."
                },
                new TipoBarrera
                {
                    Codigo = "NTC-SENALIZACION",
                    Nombre = "Señalización",
                    CriterioNorma = "Señalización visual, podotáctil y en braille que permita orientarse y advierta las situaciones de riesgo."
                },
                new TipoBarrera
                {
                    Codigo = "NTC-MOBILIARIO",
                    Nombre = "Mobiliario y áreas de atención",
                    CriterioNorma = "Mostradores a altura accesible, asientos adaptados y espacios reservados en las salas de espera."
                },
                new TipoBarrera
                {
                    Codigo = "NTC-ENTRADAS",
                    Nombre = "Auditorios, entradas y ventanillas",
                    CriterioNorma = "Accesos y puntos de atención físicamente accesibles y con la información adaptada a distintas discapacidades."
                }
            };

            await contexto.TiposBarrera.AddRangeAsync(tipos);
            await contexto.SaveChangesAsync();
        }

        /// <summary>
        /// Once lugares de Barranquilla, priorizando puntos de servicio al ciudadano,
        /// que es donde la NTC 6047 aplica y la clasificación tiene efecto jurídico.
        /// </summary>
        private static async Task SembrarLugaresAsync(AppDbContext contexto)
        {
            if (await contexto.Lugares.AnyAsync()) return;

            var tipos = await contexto.TiposBarrera.ToDictionaryAsync(t => t.Codigo);
            var hoy = DateTime.UtcNow;

            var lugares = new List<Lugar>
            {
                new Lugar
                {
                    Nombre = "Centro Administrativo Distrital (Alcaldía)",
                    Tipo = "Sede administrativa",
                    Zona = "Centro",
                    Direccion = "Calle 34 # 43-31",
                    Latitud = 10.9789,
                    Longitud = -74.7780,
                    EsServicioAlCiudadano = true,
                    Reportes =
                    {
                        new ReporteAccesibilidad
                        {
                            Usuario = "ciudadano_anonimo_12",
                            Descripcion = "La rampa de la entrada principal está partida por la mitad y casi siempre hay motos parqueadas encima, no se puede subir en silla de ruedas.",
                            FechaReporte = hoy.AddDays(-18),
                            Estado = EstadoReporte.Analizado,
                            TipoBarrera = tipos["NTC-RAMPAS"],
                            Severidad = NivelSeveridad.Alta,
                            AnalisisIa = "La descripción indica deterioro estructural de la rampa y obstrucción permanente del acceso, lo que impide el ingreso autónomo de una persona en silla de ruedas.",
                            AjusteRazonable = "Reparar la superficie de la rampa y despejar el acceso con señalización y control de parqueo en el área.",
                            CertezaIa = 0.92
                        },
                        new ReporteAccesibilidad
                        {
                            Usuario = "vecina_del_centro",
                            Descripcion = "En la ventanilla de atención al público el mostrador queda muy alto, mi mamá va en silla y le toca hablar hacia arriba, no alcanza ni a firmar los papeles.",
                            FechaReporte = hoy.AddDays(-11)
                        }
                    }
                },
                new Lugar
                {
                    Nombre = "Biblioteca Departamental Meira Delmar",
                    Tipo = "Biblioteca pública",
                    Zona = "Centro",
                    Direccion = "Calle 38 # 38-86",
                    Latitud = 10.9812,
                    Longitud = -74.7815,
                    EsServicioAlCiudadano = true,
                    Reportes =
                    {
                        new ReporteAccesibilidad
                        {
                            Usuario = "lector_frecuente",
                            Descripcion = "No hay ninguna señal en braille ni guía en el piso desde la entrada hasta la sala principal, uno tiene que pedirle a alguien que lo lleve del brazo.",
                            FechaReporte = hoy.AddDays(-25),
                            Estado = EstadoReporte.Analizado,
                            TipoBarrera = tipos["NTC-SENALIZACION"],
                            Severidad = NivelSeveridad.Media,
                            AnalisisIa = "Ausencia de señalización braille y de guía podotáctil que permita el recorrido autónomo de una persona con discapacidad visual.",
                            AjusteRazonable = "Instalar franjas podotáctiles de guía y rotulación en braille en los accesos y puntos de información.",
                            CertezaIa = 0.87
                        },
                        new ReporteAccesibilidad
                        {
                            Usuario = "estudiante_uniatlantico",
                            Descripcion = "El ascensor lleva como tres meses dañado y la sala de consulta queda en el segundo piso.",
                            FechaReporte = hoy.AddDays(-6)
                        }
                    }
                },
                new Lugar
                {
                    Nombre = "Parque Cultural del Caribe",
                    Tipo = "Museo",
                    Zona = "Centro",
                    Direccion = "Calle 36 # 46-66",
                    Latitud = 10.9905,
                    Longitud = -74.7853,
                    EsServicioAlCiudadano = true,
                    Reportes =
                    {
                        new ReporteAccesibilidad
                        {
                            Usuario = "guia_turistico_bq",
                            Descripcion = "Las salas se recorren bien pero los textos de las vitrinas están muy pequeños y no hay audio ni nada para las personas ciegas.",
                            FechaReporte = hoy.AddDays(-14)
                        }
                    }
                },
                new Lugar
                {
                    Nombre = "Terminal Metropolitana de Transporte",
                    Tipo = "Terminal",
                    Zona = "Sur",
                    Direccion = "Carrera 45 # 4-56, Prolongación Avenida Murillo",
                    Latitud = 10.8869,
                    Longitud = -74.7797,
                    EsServicioAlCiudadano = true,
                    Reportes =
                    {
                        new ReporteAccesibilidad
                        {
                            Usuario = "viajero_ocasional",
                            Descripcion = "El baño para personas con discapacidad está siempre cerrado con llave y hay que buscar a un vigilante que casi nunca aparece.",
                            FechaReporte = hoy.AddDays(-9),
                            Estado = EstadoReporte.Analizado,
                            TipoBarrera = tipos["NTC-BANOS"],
                            Severidad = NivelSeveridad.Alta,
                            AnalisisIa = "El baño accesible existe pero permanece inhabilitado en la práctica, lo que equivale a no contar con la instalación.",
                            AjusteRazonable = "Mantener el baño accesible abierto durante todo el horario de operación y asignar responsable de su aseo y control.",
                            CertezaIa = 0.9
                        },
                        new ReporteAccesibilidad
                        {
                            Usuario = "conductor_intermunicipal",
                            Descripcion = "Los parqueaderos marcados para discapacitados quedan al otro extremo de la entrada, uno cruza todo el patio de buses para llegar.",
                            FechaReporte = hoy.AddDays(-4)
                        }
                    }
                },
                new Lugar
                {
                    Nombre = "Estación Central de Transmetro",
                    Tipo = "Estación de transporte",
                    Zona = "Centro",
                    Direccion = "Avenida Olaya Herrera con Calle 30",
                    Latitud = 10.9852,
                    Longitud = -74.7861,
                    EsServicioAlCiudadano = true,
                    Reportes =
                    {
                        new ReporteAccesibilidad
                        {
                            Usuario = "usuaria_transmetro",
                            Descripcion = "El torniquete de entrada es muy angosto, con la silla de ruedas no paso y toca esperar a que abran la puerta de servicio.",
                            FechaReporte = hoy.AddDays(-20),
                            Estado = EstadoReporte.Analizado,
                            TipoBarrera = tipos["NTC-PUERTAS"],
                            Severidad = NivelSeveridad.Alta,
                            AnalisisIa = "El ancho libre del control de acceso no permite el paso de una silla de ruedas, lo que condiciona el ingreso a la asistencia de un tercero.",
                            AjusteRazonable = "Habilitar un torniquete de paso ancho operativo y señalizado en la línea de control de acceso.",
                            CertezaIa = 0.94
                        },
                        new ReporteAccesibilidad
                        {
                            Usuario = "pasajero_diario",
                            Descripcion = "En la plataforma no hay franja amarilla en relieve, un señor invidente casi se cae al vacío esperando el bus.",
                            FechaReporte = hoy.AddDays(-3)
                        }
                    }
                },
                new Lugar
                {
                    Nombre = "Portal de Transmetro Joe Arroyo",
                    Tipo = "Estación de transporte",
                    Zona = "Sur",
                    Direccion = "Avenida Murillo con Carrera 8, Soledad",
                    Latitud = 10.9165,
                    Longitud = -74.7784,
                    EsServicioAlCiudadano = true,
                    Reportes =
                    {
                        new ReporteAccesibilidad
                        {
                            Usuario = "madre_comunitaria",
                            Descripcion = "El puente peatonal para entrar al portal solo tiene escaleras y los pasamanos están sueltos, con coche de bebé o con muletas es imposible.",
                            FechaReporte = hoy.AddDays(-16)
                        },
                        new ReporteAccesibilidad
                        {
                            Usuario = "usuario_anonimo_47",
                            Descripcion = "Las taquillas no tienen ninguna pantalla ni aviso escrito, todo lo dicen por parlante y las personas sordas no se enteran de los cambios de ruta.",
                            FechaReporte = hoy.AddDays(-2)
                        }
                    }
                },
                new Lugar
                {
                    Nombre = "Plaza de la Paz Juan Pablo II",
                    Tipo = "Plaza pública",
                    Zona = "Centro",
                    Direccion = "Calle 53 con Carrera 46",
                    Latitud = 10.9930,
                    Longitud = -74.7902,
                    EsServicioAlCiudadano = false,
                    Reportes =
                    {
                        new ReporteAccesibilidad
                        {
                            Usuario = "caminante_del_centro",
                            Descripcion = "Los andenes alrededor de la plaza están levantados por las raíces de los árboles y en varios tramos hay que bajarse a la calle.",
                            FechaReporte = hoy.AddDays(-8),
                            Estado = EstadoReporte.Analizado,
                            TipoBarrera = tipos["NTC-CIRCULACION"],
                            Severidad = NivelSeveridad.Media,
                            AnalisisIa = "La superficie de circulación presenta desniveles e interrupciones que obligan a invadir la calzada vehicular.",
                            AjusteRazonable = "Nivelar los tramos afectados del andén y garantizar una ruta peatonal continua alrededor de la plaza.",
                            CertezaIa = 0.83
                        }
                    }
                },
                new Lugar
                {
                    Nombre = "Mercado de Barranquilita",
                    Tipo = "Mercado público",
                    Zona = "Centro",
                    Direccion = "Calle 8 con Carrera 38",
                    Latitud = 10.9766,
                    Longitud = -74.7723,
                    EsServicioAlCiudadano = false,
                    Reportes =
                    {
                        new ReporteAccesibilidad
                        {
                            Usuario = "comerciante_local",
                            Descripcion = "Los pasillos entre los puestos están llenos de cajas y mercancía, no queda espacio ni para caminar normal, mucho menos en silla.",
                            FechaReporte = hoy.AddDays(-13)
                        }
                    }
                },
                new Lugar
                {
                    Nombre = "Gran Malecón del Río",
                    Tipo = "Espacio público",
                    Zona = "Norte",
                    Direccion = "Vía 40, sector Puerta de Oro",
                    Latitud = 11.0185,
                    Longitud = -74.8010,
                    EsServicioAlCiudadano = false,
                    Reportes =
                    {
                        new ReporteAccesibilidad
                        {
                            Usuario = "familia_visitante",
                            Descripcion = "El recorrido principal es cómodo y plano, pero los baños del sector norte tienen un escalón en la entrada.",
                            FechaReporte = hoy.AddDays(-5)
                        }
                    }
                },
                new Lugar
                {
                    Nombre = "Estadio Metropolitano Roberto Meléndez",
                    Tipo = "Escenario deportivo",
                    Zona = "Sur",
                    Direccion = "Avenida Circunvalar con Calle 45",
                    Latitud = 10.9258,
                    Longitud = -74.8008,
                    EsServicioAlCiudadano = false,
                    Reportes =
                    {
                        new ReporteAccesibilidad
                        {
                            Usuario = "hincha_junior_1948",
                            Descripcion = "Los baños adaptados los cierran los días de partido y los usan como bodega, justo cuando más gente hay.",
                            FechaReporte = hoy.AddDays(-22)
                        },
                        new ReporteAccesibilidad
                        {
                            Usuario = "acompanante_familiar",
                            Descripcion = "Las sillas para personas en silla de ruedas quedan detrás de la baranda publicitaria y no se ve nada de la cancha.",
                            FechaReporte = hoy.AddDays(-7)
                        }
                    }
                },
                new Lugar
                {
                    Nombre = "Universidad del Atlántico, sede norte",
                    Tipo = "Universidad pública",
                    Zona = "Norte",
                    Direccion = "Kilómetro 7 antigua vía a Puerto Colombia",
                    Latitud = 11.0198,
                    Longitud = -74.8722,
                    EsServicioAlCiudadano = true,
                    Reportes =
                    {
                        new ReporteAccesibilidad
                        {
                            Usuario = "estudiante_ingenieria",
                            Descripcion = "El auditorio principal solo se entra por escaleras, los compañeros en silla se quedan afuera en las conferencias.",
                            FechaReporte = hoy.AddDays(-10)
                        }
                    }
                }
            };

            await contexto.Lugares.AddRangeAsync(lugares);
            await contexto.SaveChangesAsync();
        }
    }
}
