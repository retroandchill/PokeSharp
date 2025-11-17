using PokeSharp.Core;
using PokeSharp.Data.Pbs;

namespace PokeSharp.PokemonModel.Forms;

public static class MegaEvolutionExtensions
{
    extension(Pokemon pokemon)
    {
        # region Mega Evolution
        public int? MegaForm
        {
            get
            {
                foreach (var data in Species.Entities)
                {
                    if (data.SpeciesId != pokemon.Species || data.UnmegaForm != pokemon.FormSimple)
                        continue;

                    if (
                        data.MegaStone.IsValid && pokemon.HasSpecificItem(data.MegaStone)
                        || data.MegaMove.IsValid && pokemon.HasMove(data.MegaMove)
                    )
                    {
                        return data.Form;
                    }
                }

                return -1;
            }
        }

        public int? UnmegaForm => pokemon.IsMega ? pokemon.SpeciesData.UnmegaForm : null;

        public bool HasMegaForm
        {
            get
            {
                var megaForm = pokemon.MegaForm;
                return megaForm.HasValue && megaForm != pokemon.FormSimple;
            }
        }

        public bool IsMega
        {
            get
            {
                var speciesData = pokemon.SpeciesData;
                return speciesData.MegaStone.IsValid || speciesData.MegaMove.IsValid;
            }
        }

        public void MakeMega()
        {
            var megaForm = pokemon.MegaForm;
            if (megaForm.HasValue)
            {
                pokemon.Form = megaForm.Value;
            }
        }

        public void MakeUnmega()
        {
            var unmegaForm = pokemon.UnmegaForm;
            if (unmegaForm.HasValue)
            {
                pokemon.Form = unmegaForm.Value;
            }
        }

        public Text MegaName
        {
            get
            {
                var speciesData = pokemon.SpeciesData;
                var formName = speciesData.FormName;

                // TODO: Make this configurable
                return formName ?? $"Mega {speciesData.Name}";
            }
        }

        public MegaMessageType MegaMessageType
        {
            get
            {
                var megaForm = pokemon.MegaForm;
                return megaForm.HasValue
                    ? Species.Get(pokemon.Species, megaForm.Value).MegaMessage
                    : MegaMessageType.Normal;
            }
        }

        #endregion

        #region Primal Reversion

        public bool HasPrimalForm => MultipleForms.GetPrimalForm(pokemon).HasValue;

        public bool IsPrimal => MultipleForms.GetPrimalForm(pokemon) == pokemon.FormSimple;

        public void MakePrimal()
        {
            var primalForm = MultipleForms.GetPrimalForm(pokemon);
            if (primalForm.HasValue)
            {
                pokemon.Form = primalForm.Value;
            }
        }

        public void MakeUnprimal()
        {
            var primalForm = MultipleForms.GetPrimalForm(pokemon);
            if (primalForm.HasValue)
            {
                pokemon.Form = primalForm.Value;
            }
            else if (pokemon.IsPrimal)
            {
                pokemon.Form = 0;
            }
        }

        #endregion
    }
}
