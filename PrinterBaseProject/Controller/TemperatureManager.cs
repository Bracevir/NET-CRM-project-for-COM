using System.Collections.Generic;

public class ExtruderData
{
    public float currentTemperature { get; set; }
    public float signalTemperature { get; set; }
    public bool signalSend { get; set; }

    public ExtruderData(float signalTemp)
    {
        signalTemperature = signalTemp;
        signalSend = false;
        currentTemperature = 0f;
    }
}

public class TemperatureManager
{
    private readonly Dictionary<int, float> extruderTemp = new Dictionary<int, float>();
    private readonly Dictionary<int, ExtruderData> extruderData = new Dictionary<int, ExtruderData>();
    public int ActiveExtruderId { get; set; } // Хранит ID активного экструдерa

    public TemperatureManager()
    {
        ActiveExtruderId = 0; // Предположим, что экструдер с ID 0 активен по умолчанию
        // Инициализация данных экструдеров с сигнализацией при достижении температуры 200°C
        extruderData[0] = new ExtruderData(200f);
        extruderData[1] = new ExtruderData(200f); // Пример для второго экструдерa
    }

    /// <summary>
    /// Получает температуру экструдерa по его ID.
    /// </summary>
    public float GetTemperature(int extr)
    {
        if (extr < 0)
        {
            extr = ActiveExtruderId;
        }

        lock (extruderTemp)
        {
            if (!extruderTemp.ContainsKey(extr))
            {
                extruderTemp[extr] = 0f; // Инициализация температуры, если ее нет
            }
            return extruderTemp[extr];
        }
    }

    /// <summary>
    /// Устанавливает температуру экструдерa по его ID.
    /// </summary>
    public void SetTemperature(int extr, float t)
    {
        if (extr < 0)
        {
            extr = ActiveExtruderId;
        }

        lock (extruderTemp)
        {
            if (!extruderTemp.ContainsKey(extr))
            {
                extruderTemp.Add(extr, t);
            }
            else
            {
                extruderTemp[extr] = t;
            }

            // Обновление текущей температуры в данных экструдеров
            if (extruderData.ContainsKey(extr))
            {
                extruderData[extr].currentTemperature = t;

                // Проверка на достижение сигнализируемой температуры
                if (!extruderData[extr].signalSend && t >= extruderData[extr].signalTemperature)
                {
                    extruderData[extr].signalSend = true;
                    // Здесь можно добавить вызов звукового сигнала или другого действия
                    // Например: SoundManager.PlayTemperature(false);
                }
            }
        }
    }
}