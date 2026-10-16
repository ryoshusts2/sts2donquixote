using DonQuixote.Chains;
using DonQuixote.Characters;
using STS2RitsuLib.Interop.AutoRegistration;

namespace DonQuixote.Cards;

[RegisterCard(typeof(DonQuixoteCardPool))]
public sealed class FirstWound : ChainLinkBase
{
    protected override int ChainIndex => 6;
}