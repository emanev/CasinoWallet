using CasinoWallet.Configuration;
using CasinoWallet.Exceptions;

namespace CasinoWallet
{
    public static class GameSettingsValidator
    {
        public static void Validate(GameSettings? settings)
        {
            if (settings == null)
                throw new InvalidSettingsException("Missing GameSettings section in configuration.");

            if (settings.MinBet <= 0)
                throw new InvalidSettingsException("GameSettings.MinBet is missing or invalid.");

            if (settings.MaxBet <= 0)
                throw new InvalidSettingsException("GameSettings.MaxBet is missing or invalid.");

            if (settings.MinBet > settings.MaxBet)
                throw new InvalidSettingsException("MinBet cannot be greater than MaxBet.");

            int totalChance = settings.WinChances.Lose + settings.WinChances.WinLow + settings.WinChances.WinHigh;
            if (totalChance != 100)
                throw new InvalidSettingsException("Total WinChances must equal 100%.");

            if (settings.Multipliers.WinLowMin <= 0 || settings.Multipliers.WinLowMax <= 0)
                throw new InvalidSettingsException("WinLow multipliers must be positive.");

            if (settings.Multipliers.WinHighMin <= 0 || settings.Multipliers.WinHighMax <= 0)
                throw new InvalidSettingsException("WinHigh multipliers must be positive.");

            if (settings.Multipliers.WinLowMin >= settings.Multipliers.WinLowMax)
                throw new InvalidSettingsException("WinLowMin must be less than WinLowMax.");

            if (settings.Multipliers.WinHighMin >= settings.Multipliers.WinHighMax)
                throw new InvalidSettingsException("WinHighMin must be less than WinHighMax.");

            if (settings.Multipliers.WinLowMax >= settings.Multipliers.WinHighMin)
                throw new InvalidSettingsException("WinLowMax must be less than WinHighMin to avoid overlap.");
        }
    }
}